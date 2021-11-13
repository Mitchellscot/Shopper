using Microsoft.Extensions.Options;

namespace shopper.tests.helpers
{
    public class TestOptionsSnapshot<T> : IOptionsSnapshot<T> where T : class
    {
        private T _settings;

        public TestOptionsSnapshot(T settings)
            => (_settings) = (settings);

        public T Value => _settings;
        public T Get(string name) => throw new System.NotImplementedException();
    }
}