namespace HotelWebApp.Models
{
    /// <summary>
    /// ViewModel that represents a batch of changes sent from the Syncfusion Scheduler component.
    /// The Scheduler sends all created, updated, and deleted events in a single request,
    /// which is bound to this model.
    /// </summary>
    public class SchedulerBatchUpdateViewModel
    {
        /// <summary>
        /// A list of new event objects that were added on the client-side scheduler.
        /// This will be null if no events were added.
        /// </summary>
        public List<SchedulerEventViewModel>? Added { get; set; }

        /// <summary>
        /// A list of event objects that were modified (e.g., dragged or resized) on the client-side scheduler.
        /// This will be null if no events were changed.
        /// </summary>
        public List<SchedulerEventViewModel>? Changed { get; set; }

        /// <summary>
        /// A list of event objects that were deleted on the client-side scheduler.
        /// In our current implementation, only the first item in this list is used,
        /// as the delete action is performed on a single event at a time.
        /// </summary>
        public List<SchedulerEventViewModel>? Deleted { get; set; }

        /// <summary>
        /// An optional dictionary for any additional custom parameters sent with the batch request.
        /// </summary>
        public IDictionary<string, object>? Params { get; set; }
    }
}
