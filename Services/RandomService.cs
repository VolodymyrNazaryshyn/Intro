namespace Intro.Services 
{
    public class RandomService 
    {
        private System.Random random = new();

        public int Integer => random.Next();
    }
}
