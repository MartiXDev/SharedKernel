# MartiX.SharedKernel Library

A Domain-Driven Design (DDD) shared kernel library for .NET 10.0, providing base classes and interfaces for building domain models with domain events, entities, value objects, and repository patterns.

## Overview

This library provides foundational building blocks for Clean Architecture and DDD applications, including:

- **Domain Events** - Capture and dispatch significant domain occurrences
- **Entity Base Classes** - Identity-based domain objects with event support
- **Value Objects** - Immutable objects compared by value, not identity
- **Repository Patterns** - Abstractions for data access
- **MediatR Integration** - Event dispatching via MediatR pipeline

## Projects

### MartiX.SharedKernel

Main library containing core DDD building blocks, targeting .NET 10.0.

### MartiX.SharedKernel.UnitTests

Comprehensive xUnit test suite with Moq, Shouldly, and AwesomeAssertions.

## Core Interfaces

### Domain Model Interfaces

- **`IAggregateRoot`** - Marker interface identifying aggregate roots (consistency boundaries in DDD)
- **`IDomainEvent`** - Base interface for all domain events representing significant domain occurrences
- **`IHasDomainEvents`** - Exposes `DomainEvents` collection and `ClearDomainEvents()` method for entities that raise events

### Event Handling Interfaces

- **`IDomainEventHandler<TDomainEvent>`** - Generic handler interface for processing specific domain event types
- **`IDomainEventDispatcher`** - Responsible for publishing domain events from entities to handlers

### Data Access Interfaces

- **`IReadRepository<T>`** - Read-only repository pattern for queries without mutation
- **`IRepository<T>`** - Full repository pattern with CRUD operations for aggregate roots

## Base Classes

### `DomainEventBase`

Abstract base class for domain events implementing `IDomainEvent`. Provides common event metadata such as timestamp when the event occurred.

### `EntityBase`

Abstract base class for entities with:

- Identity via `Id` property
- Domain event tracking via `IHasDomainEvents`
- `AddDomainEvent()` method to queue events on the entity

### `HasDomainEventsBase`

Abstract base class managing domain events:

- Maintains the `DomainEvents` collection
- Implements `ClearDomainEvents()` to reset event queue
- Used by entities that need to track and raise domain events

### `ValueObject`

Abstract base class for value objects in DDD:

- Immutable objects compared by their properties, not identity
- Overrides `Equals()`, `GetHashCode()`, and operators `==`/`!=`
- Ensures structural equality (two objects with identical values are equal)

## Infrastructure & Integration

### `MediatorDomainEventDispatcher`

Implements `IDomainEventDispatcher` using **MediatR**:

- Discovers entities with queued domain events
- Publishes events through the MediatR pipeline
- Supports various ID types:
  - `Guid`-based entity IDs
  - Strongly-typed custom IDs (e.g., `OrderId`, `CustomerId`)
  - Mixed ID types within the same aggregate

### `LoggingBehavior`

MediatR pipeline behavior for cross-cutting logging:

- Logs requests and responses passing through the mediator
- Useful for debugging, monitoring, and audit trails

## Key Patterns & Design Principles

1. **Domain Events Pattern** - Entities raise events when state changes; dispatcher publishes them via MediatR for decoupled handling
2. **Aggregate Root Pattern** - `IAggregateRoot` marks consistency and transactional boundaries
3. **Value Object Pattern** - Immutable objects with structural equality, perfect for domain concepts like Money, Address, etc.
4. **Repository Pattern** - Clean data access abstractions separating domain from infrastructure
5. **MediatR Integration** - CQRS-ready with built-in support for cross-cutting concerns
6. **Strongly-Typed IDs** - Support for domain-specific ID types beyond primitive `Guid`/`int`

## Usage Example

### Multi-Domain E-Commerce Scenario

This example demonstrates how aggregates work across Order, Inventory, and Customer domains with domain events facilitating cross-domain communication.

```csharp
// ==================== VALUE OBJECTS ====================

// Shared value object - Money
public class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}

// Shared value object - Address
public class Address : ValueObject
{
    public string Street { get; }
    public string City { get; }
    public string PostalCode { get; }
    public string Country { get; }

    public Address(string street, string city, string postalCode, string country)
    {
        Street = street;
        City = city;
        PostalCode = postalCode;
        Country = country;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return PostalCode;
        yield return Country;
    }
}

// ==================== ORDER DOMAIN ====================

// Domain Events
public class OrderPlacedEvent : DomainEventBase
{
    public Guid OrderId { get; }
    public Guid CustomerId { get; }
    public IReadOnlyList<OrderItem> Items { get; }

    public OrderPlacedEvent(Guid orderId, Guid customerId, IReadOnlyList<OrderItem> items)
    {
        OrderId = orderId;
        CustomerId = customerId;
        Items = items;
    }
}

public class OrderShippedEvent : DomainEventBase
{
    public Guid OrderId { get; }
    public Address ShippingAddress { get; }
    public DateTime ShippedDate { get; }

    public OrderShippedEvent(Guid orderId, Address shippingAddress, DateTime shippedDate)
    {
        OrderId = orderId;
        ShippingAddress = shippingAddress;
        ShippedDate = shippedDate;
    }
}

// Order Aggregate Root
public class Order : EntityBase, IAggregateRoot
{
    private readonly List<OrderItem> _items = new();
    
    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public Money TotalAmount { get; private set; }
    public Address ShippingAddress { get; private set; }
    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

    private Order() { } // For EF Core

    public static Order Create(Guid customerId, Address shippingAddress)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            ShippingAddress = shippingAddress,
            Status = OrderStatus.Draft
        };
        return order;
    }

    public void AddItem(Guid productId, int quantity, Money unitPrice)
    {
        if (Status != OrderStatus.Draft)
            throw new InvalidOperationException("Cannot add items to a non-draft order");

        var item = new OrderItem(productId, quantity, unitPrice);
        _items.Add(item);
        RecalculateTotal();
    }

    public void PlaceOrder()
    {
        if (Status != OrderStatus.Draft)
            throw new InvalidOperationException("Order already placed");
        
        if (!_items.Any())
            throw new InvalidOperationException("Cannot place order without items");

        Status = OrderStatus.Placed;
        AddDomainEvent(new OrderPlacedEvent(Id, CustomerId, Items));
    }

    public void Ship()
    {
        if (Status != OrderStatus.Placed)
            throw new InvalidOperationException("Only placed orders can be shipped");

        Status = OrderStatus.Shipped;
        AddDomainEvent(new OrderShippedEvent(Id, ShippingAddress, DateTime.UtcNow));
    }

    private void RecalculateTotal()
    {
        var total = _items.Sum(i => i.TotalPrice.Amount);
        TotalAmount = new Money(total, "USD");
    }
}

// Entity within Order aggregate
public class OrderItem : EntityBase
{
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; }
    public Money TotalPrice => new Money(Quantity * UnitPrice.Amount, UnitPrice.Currency);

    internal OrderItem(Guid productId, int quantity, Money unitPrice)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}

public enum OrderStatus { Draft, Placed, Shipped, Delivered, Cancelled }

// ==================== INVENTORY DOMAIN ====================

// Domain Events
public class InventoryReservedEvent : DomainEventBase
{
    public Guid ProductId { get; }
    public int Quantity { get; }
    public Guid OrderId { get; }

    public InventoryReservedEvent(Guid productId, int quantity, Guid orderId)
    {
        ProductId = productId;
        Quantity = quantity;
        OrderId = orderId;
    }
}

public class LowStockDetectedEvent : DomainEventBase
{
    public Guid ProductId { get; }
    public int CurrentStock { get; }
    public int ReorderThreshold { get; }

    public LowStockDetectedEvent(Guid productId, int currentStock, int reorderThreshold)
    {
        ProductId = productId;
        CurrentStock = currentStock;
        ReorderThreshold = reorderThreshold;
    }
}

// Product Aggregate Root
public class Product : EntityBase, IAggregateRoot
{
    public string Name { get; private set; }
    public int StockQuantity { get; private set; }
    public int ReorderThreshold { get; private set; }
    public Money Price { get; private set; }

    private Product() { } // For EF Core

    public static Product Create(string name, Money price, int initialStock, int reorderThreshold)
    {
        return new Product
        {
            Id = Guid.NewGuid(),
            Name = name,
            Price = price,
            StockQuantity = initialStock,
            ReorderThreshold = reorderThreshold
        };
    }

    public void ReserveStock(int quantity, Guid orderId)
    {
        if (StockQuantity < quantity)
            throw new InvalidOperationException($"Insufficient stock for {Name}");

        StockQuantity -= quantity;
        AddDomainEvent(new InventoryReservedEvent(Id, quantity, orderId));

        if (StockQuantity <= ReorderThreshold)
        {
            AddDomainEvent(new LowStockDetectedEvent(Id, StockQuantity, ReorderThreshold));
        }
    }

    public void RestockInventory(int quantity)
    {
        StockQuantity += quantity;
    }
}

// ==================== CUSTOMER DOMAIN ====================

// Domain Events
public class CustomerRewardPointsEarnedEvent : DomainEventBase
{
    public Guid CustomerId { get; }
    public int PointsEarned { get; }
    public Guid OrderId { get; }

    public CustomerRewardPointsEarnedEvent(Guid customerId, int pointsEarned, Guid orderId)
    {
        CustomerId = customerId;
        PointsEarned = pointsEarned;
        OrderId = orderId;
    }
}

// Customer Aggregate Root
public class Customer : EntityBase, IAggregateRoot
{
    public string Name { get; private set; }
    public string Email { get; private set; }
    public int RewardPoints { get; private set; }

    private Customer() { } // For EF Core

    public static Customer Create(string name, string email)
    {
        return new Customer
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            RewardPoints = 0
        };
    }

    public void AddRewardPoints(int points, Guid orderId)
    {
        RewardPoints += points;
        AddDomainEvent(new CustomerRewardPointsEarnedEvent(Id, points, orderId));
    }
}

// ==================== EVENT HANDLERS (Cross-Domain Coordination) ====================

// When an order is placed, reserve inventory for each item
public class OrderPlacedInventoryReservationHandler : IDomainEventHandler<OrderPlacedEvent>
{
    private readonly IRepository<Product> _productRepository;

    public OrderPlacedInventoryReservationHandler(IRepository<Product> productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task Handle(OrderPlacedEvent domainEvent, CancellationToken cancellationToken)
    {
        foreach (var item in domainEvent.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);
            product.ReserveStock(item.Quantity, domainEvent.OrderId);
            await _productRepository.UpdateAsync(product, cancellationToken);
        }
    }
}

// When an order is shipped, award reward points to the customer
public class OrderShippedRewardPointsHandler : IDomainEventHandler<OrderShippedEvent>
{
    private readonly IRepository<Customer> _customerRepository;
    private readonly IReadRepository<Order> _orderRepository;

    public OrderShippedRewardPointsHandler(
        IRepository<Customer> customerRepository,
        IReadRepository<Order> orderRepository)
    {
        _customerRepository = customerRepository;
        _orderRepository = orderRepository;
    }

    public async Task Handle(OrderShippedEvent domainEvent, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(domainEvent.OrderId, cancellationToken);
        var customer = await _customerRepository.GetByIdAsync(order.CustomerId, cancellationToken);
        
        // Award 1 point per $10 spent
        var points = (int)(order.TotalAmount.Amount / 10);
        customer.AddRewardPoints(points, order.Id);
        
        await _customerRepository.UpdateAsync(customer, cancellationToken);
    }
}

// When low stock is detected, notify purchasing team
public class LowStockNotificationHandler : IDomainEventHandler<LowStockDetectedEvent>
{
    private readonly IEmailService _emailService;

    public LowStockNotificationHandler(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task Handle(LowStockDetectedEvent domainEvent, CancellationToken cancellationToken)
    {
        await _emailService.SendAsync(
            to: "purchasing@company.com",
            subject: "Low Stock Alert",
            body: $"Product {domainEvent.ProductId} has only {domainEvent.CurrentStock} units remaining."
        );
    }
}

// ==================== USAGE IN APPLICATION LAYER ====================

public class PlaceOrderCommand
{
    public Guid CustomerId { get; set; }
    public Address ShippingAddress { get; set; }
    public List<OrderItemDto> Items { get; set; }
}

public class PlaceOrderCommandHandler
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IDomainEventDispatcher _eventDispatcher;

    public PlaceOrderCommandHandler(
        IRepository<Order> orderRepository,
        IDomainEventDispatcher eventDispatcher)
    {
        _orderRepository = orderRepository;
        _eventDispatcher = eventDispatcher;
    }

    public async Task<Guid> Handle(PlaceOrderCommand command, CancellationToken cancellationToken)
    {
        // Create order aggregate
        var order = Order.Create(command.CustomerId, command.ShippingAddress);

        // Add items
        foreach (var item in command.Items)
        {
            order.AddItem(item.ProductId, item.Quantity, new Money(item.UnitPrice, "USD"));
        }

        // Place the order (raises OrderPlacedEvent)
        order.PlaceOrder();

        // Persist
        await _orderRepository.AddAsync(order, cancellationToken);

        // Dispatch domain events (triggers InventoryReservationHandler, etc.)
        await _eventDispatcher.DispatchAndClearEvents(order);

        return order.Id;
    }
}
```

### Key Takeaways from This Example

1. **Aggregate Boundaries** - Each aggregate (Order, Product, Customer) is a consistency boundary with its own repository
2. **Value Objects** - `Money` and `Address` are immutable and compared by value across all domains
3. **Domain Events** - Aggregates raise events when important state changes occur
4. **Cross-Domain Communication** - Event handlers coordinate between aggregates (OrderPlaced → Reserve Inventory → Award Points)
5. **Eventual Consistency** - Changes across aggregates happen via events, not direct calls
6. **Single Transaction Per Aggregate** - Each aggregate is saved independently; events ensure cross-aggregate consistency

## Make Your Own

You should probably fork or copy the classes in this repo and create your own `Acme.SharedKernel` package! Host it on [NuGet](https://www.nuget.org/) or better yet, your own private package feed. This is not a supported package and is really only intended as a demo for use with the [Clean Architecture Solution Template](https://www.nuget.org/packages/Ardalis.CleanArchitecture.Template/).

To see an example of what *you* should do, check out the [sample folder in the Clean Architecture repo](https://github.com/ardalis/CleanArchitecture/tree/main/sample) and notice that it depends on its own, separate [NimblePros.SharedKernel](https://github.com/NimblePros/SharedKernelSample) package.

## References

- [Clean Architecture Template GitHub Repo](https://github.com/ardalis/cleanarchitecture)
- [Domain-Driven Design Fundamentals](https://www.pluralsight.com/courses/domain-driven-design-fundamentals)
- [C# Record Struct for Value Objects](https://nietras.com/2021/06/14/csharp-10-record-struct/)
