namespace ProRental.Domain.Entities;

public partial class Train
{
    private int getTrainID() => _trainId;

    private void setTrainID(int trainId)
    {
        _trainId = trainId;
    }

    private string getTrainType() => _trainType ?? string.Empty;

    private void setTrainType(string trainType)
    {
        _trainType = trainType;
    }

    private string getTrainNumber() => _trainNumber ?? string.Empty;

    private void setTrainNumber(string trainNumber)
    {
        _trainNumber = trainNumber;
    }

    public int ReadTrainId() => getTrainID();

    public string ReadTrainType() => getTrainType();

    public string ReadTrainNumber() => getTrainNumber();
}
