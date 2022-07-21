namespace Intro.Services
{
    public class CurrentDateTime : IDateTime
    {
        public string Date()
        {
            var date = System.DateTime.Now.ToString("dd MMMM yyyy г");
            return date;
        }
        public string Time()
        {
            var time = System.DateTime.Now.ToString("HH:mm:ss");
            return time;
        }
    }
}
