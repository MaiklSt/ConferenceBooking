namespace ConferenceBooking.Domain.Entities;

/// <summary>
/// Конференц-зал, який компанія здає в оренду.
/// </summary>
public class Room
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int Capacity { get; private set; }
    public decimal BaseHourlyRate { get; private set; }

    private readonly List<Service> _availableServices = [];
    public IReadOnlyCollection<Service> AvailableServices => _availableServices.AsReadOnly();

    private Room() { }

    public Room(string name, int capacity, decimal baseHourlyRate, IEnumerable<Service>? services = null)
    {
        Id = Guid.NewGuid();
        UpdateDetails(name, capacity, baseHourlyRate);

        if (services is not null)
        {
            _availableServices.AddRange(services);
        }
    }

    public void UpdateDetails(string name, int capacity, decimal baseHourlyRate)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Назва залу не може бути порожньою.", nameof(name));

        if (capacity <= 0)
            throw new ArgumentException("Місткість має бути більше нуля.", nameof(capacity));

        if (baseHourlyRate <= 0)
            throw new ArgumentException("Базова вартість має бути більше нуля.", nameof(baseHourlyRate));

        Name = name;
        Capacity = capacity;
        BaseHourlyRate = baseHourlyRate;
    }

    public void AddService(Service service)
    {
        ArgumentNullException.ThrowIfNull(service);

        if (_availableServices.Any(s => s.Id == service.Id))
            return;

        _availableServices.Add(service);
    }

    public void RemoveService(Guid serviceId)
    {
        _availableServices.RemoveAll(s => s.Id == serviceId);
    }

    public bool HasService(Guid serviceId) =>
        _availableServices.Any(s => s.Id == serviceId);
}
