namespace UfoData;

public interface IParser
{
    Task<IList<Sighting>> Parse(Stream stream);
}