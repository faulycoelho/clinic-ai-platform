namespace Clinic.Application.Services
{
    public class ChunkService
    {
        public List<string> Split(string text, int size = 500)
        {
            var chunks = new List<string>();

            for (int i = 0; i < text.Length; i += size)
            {
                chunks.Add(text.Substring(i, Math.Min(size, text.Length - i)));
            }

            return chunks;
        }
    }
}
