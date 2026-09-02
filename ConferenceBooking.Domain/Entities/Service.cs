namespace ConferenceBooking.Domain.Entities;

/// <summary>
/// Додаткова послуга, яку можна замовити разом із орендою залу (проєктор, Wi-Fi тощо).
/// </summary>
public class Service
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public decimal Price { get; private set; }

    private Service() { }

    public Service(string name, decimal price)
    {
        Id = Guid.NewGuid();
        Name = name;
        Price = price;
    }

    public void Update(string name, decimal price)
    {
        Name = name;
        Price = price;
    }
}
