namespace MO_QAP
{
    public class CancelationToken
    {
        private bool cancelationPending = false;
        public void Cancel()
        {
            cancelationPending = true;
        }

        public bool IsCancelationPending()
        {
            return this.cancelationPending;
        }
    }
}