
using Microsoft.Data.SqlClient;
// using ZooWeb.Data;
using ZooWeb.Data.Entities;

namespace ZooWeb.Data{
  public class FeedingReportRepository{

    private readonly IDbConnectionFactory _factory;

    public FeedingReportRepository(IDbConnectionFactory factory)
    {
      _factory = factory;
    }

    // An animal can have more than one feeding pattern, so we return a List
    public async Task<List<FeedingReportInfo>> GetByIdAsync(int id){
      List<FeedingReportInfo> feeding_reports = new List<FeedingReportInfo>();
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();

      command.CommandText = """
        SELECT animal.Animal_ID, Name, Meal, Portion, Schedule_days, Schedule_time, Status, Location_ID
        FROM feeding_pattern, animal
        WHERE animal.Animal_Id = @id AND feeding_pattern.Animal_Id = @id;
      """;

      command.Parameters.AddWithValue("@id", id);

      await using var reader = await command.ExecuteReaderAsync();

      while (await reader.ReadAsync()){
        FeedingReportInfo entity = new FeedingReportInfo{
          AnimalId = reader.GetInt32(reader.GetOrdinal("animal.Animal_ID")).ToString(),
          Name = reader.GetString(reader.GetOrdinal("Name")),
          Meal = reader.GetString(reader.GetOrdinal("Meal")),
          Portion = reader.GetDecimal(reader.GetOrdinal("Portion")).ToString(),
          ScheduleDays = reader.GetString(reader.GetOrdinal("Schedule_days")),
          ScheduleTime = reader.GetTimeSpan(reader.GetOrdinal("Schedule_time")).ToString(),
          Status = reader.GetString(reader.GetOrdinal("Status")),
          LocationId = reader.GetInt64(reader.GetOrdinal("Location_ID")).ToString()
        };
        feeding_reports.Add(entity);
      }
      return feeding_reports;
    }

    public async Task<List<FeedingReportInfo>> GetAllAsync(){
      List<FeedingReportInfo> feeding_reports = new List<FeedingReportInfo>();
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();

      command.CommandText = """
        SELECT animal.Animal_ID AS AnimalId, Name, Meal, Portion, Schedule_days, Schedule_time, Status, Location_ID
        FROM feeding_pattern
        INNER JOIN animal
        ON animal.Animal_ID = feeding_pattern.Animal_ID
        ORDER BY animal.Animal_ID
        """;

      await using var reader = await command.ExecuteReaderAsync();

      while (await reader.ReadAsync()){
        FeedingReportInfo entity = new FeedingReportInfo{
          AnimalId = reader.GetInt32(reader.GetOrdinal("AnimalId")).ToString(),
          Name = reader.GetString(reader.GetOrdinal("Name")),
          Meal = reader.GetString(reader.GetOrdinal("Meal")),
          Portion = reader.GetDecimal(reader.GetOrdinal("Portion")).ToString(),
          ScheduleDays = reader.GetString(reader.GetOrdinal("Schedule_days")),
          ScheduleTime = reader.GetTimeSpan(reader.GetOrdinal("Schedule_time")).ToString(),
          Status = reader.GetString(reader.GetOrdinal("Status")),
          LocationId = reader.GetInt64(reader.GetOrdinal("Location_ID")).ToString()
        };
        feeding_reports.Add(entity);
      }
      return feeding_reports;
    }
  }
}
