namespace ProRental.Domain.Entities;

public partial class Packagingmaterial
{
    public string ReadMaterialName()
    {
        return Name;
}

    public string ReadMaterialType()
    {
        return Type ?? string.Empty;
    }
}