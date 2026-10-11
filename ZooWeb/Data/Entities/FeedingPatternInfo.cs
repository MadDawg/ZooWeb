namespace ZooWeb.Data.Entities{
  public class FeedingPatternInfo
  {
    public string AnimalId;
    public string Meal;
    public string Portion;
    public List<string> ScheduleDays;
    public TimeSpan ScheduleTime;
  }
}
