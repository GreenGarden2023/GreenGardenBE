namespace GreeenGarden.Business.Utilities.Convert
{
    public class ConvertUtil
    {
        public static DateTime convertStringToDateTime(string data)
        {
            return DateTime.ParseExact(data, "dd/MM/yyyy", null);
        }
    }
}
