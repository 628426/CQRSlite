using System.Threading.Tasks;
using CQRSlite.Bus;
using CQRSlite.Domain.Exception;
using CQRSlite.Tests.Substitutes;
using Xunit;

namespace CQRSlite.Tests.Bus
{
    public class When_publishing_events
    {
        private InProcessBus _bus;

        public When_publishing_events()
        {
            _bus = new InProcessBus();
        }
		
        [Fact]
        public async Task Should_publish_to_all_handlers()
        {
            var handler = new TestAggregateDidSomethingHandler();
            _bus.RegisterHandler<TestAggregateDidSomething>(handler.Handle);
            _bus.RegisterHandler<TestAggregateDidSomething>(handler.Handle);
            await _bus.Publish(new TestAggregateDidSomething());
            Assert.Equal(2, handler.TimesRun);
        }

        [Fact]
        public async Task Should_work_with_no_handlers()
        {
            await _bus.Publish(new TestAggregateDidSomething());
        }

        [Fact]
        public async Task ShouldThrowIfHandlerThrows()
        {
            var handler = new TestAggregateDidSomethingHandler();
            _bus.RegisterHandler<TestAggregateDidSomething>(handler.Handle);
            await Assert.ThrowsAsync<ConcurrencyException>(
                async () => await _bus.Publish(new TestAggregateDidSomething {Version = -10}));

        }
    }
}