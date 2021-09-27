using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Exercise3.FrontDeskAggregate;
using Exercise3.OrderAggregate;

namespace Exercise3.ChefAggregate
{
    public class Chef
    {
        public Chef(FrontDesk frontDesk, IChefActions chefActions)
        {
        }

        public void StartProcessingOrders(CancellationToken cancellationToken)
        {
            //Task.Factory.StartNew invokes an action in a new thread so that the calculation does not block the UI thread (otherwise the UI would hang)
            Task.Factory.StartNew(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    //TODO: check if the queue contains an order. If so -> process it.
                }
            }, cancellationToken);
        }
    }
}