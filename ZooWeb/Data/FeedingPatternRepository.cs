using Microsoft.Data.SqlClient;
// using ZooWeb.Data;
using ZooWeb.Data.Entities;

namespace ZooWeb.Data{
  public class FeedingPatternRepository{

    private readonly IDbConnectionFactory _factory;

    public FeedingPatternRepository(IDbConnectionFactory factory)
    {
      _factory = factory;
    }

    public async Task<FeedingPatternInfo?> GetByIdAsync(int id){
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();

      command.CommandText = """
        SELECT *
        FROM feeding_pattern
        WHERE Animal_ID = @id
        """;

      command.Parameters.AddWithValue("@id", id);

      await using var reader = await command.ExecuteReaderAsync();
      if (!await reader.ReadAsync()){ return null; }

      return new FeedingPatternInfo{
        AnimalId = reader.GetInt32(reader.GetOrdinal("Animal_ID")).ToString(),
        Meal = reader.GetString(reader.GetOrdinal("Meal")),
        Portion = reader.GetDecimal(reader.GetOrdinal("Portion")).ToString(),
        // ScheduleDays = reader.GetString(reader.GetOrdinal("Schedule_days")),
        // ScheduleTime = reader.GetTimeSpan(reader.GetOrdinal("Schedule_time")).ToString(),
        ScheduleDays = reader.GetString(reader.GetOrdinal("Schedule_days")).Split(',').ToList(),
        ScheduleTime = reader.GetTimeSpan(reader.GetOrdinal("Schedule_time"))
      };
    }

    public async Task<List<FeedingPatternInfo>> GetAllAsync(){
      List<FeedingPatternInfo> feeding_patterns = new List<FeedingPatternInfo>();
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();

      command.CommandText = """
        SELECT *
        FROM feeding_pattern
        """;

      await using var reader = await command.ExecuteReaderAsync();

      while (await reader.ReadAsync()){
        FeedingPatternInfo entity = new FeedingPatternInfo{
          AnimalId = reader.GetInt32(reader.GetOrdinal("Animal_ID")).ToString(),
          Meal = reader.GetString(reader.GetOrdinal("Meal")),
          Portion = reader.GetDecimal(reader.GetOrdinal("Portion")).ToString(),
          ScheduleDays = reader.GetString(reader.GetOrdinal("Schedule_days")).Split(',').ToList(),
          ScheduleTime = reader.GetTimeSpan(reader.GetOrdinal("Schedule_time"))
        };
        feeding_patterns.Add(entity);
      }
      return feeding_patterns;
    }

    public async Task AddAsync(FeedingPatternInfo entity){
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();

      command.CommandText = """
        INSERT INTO feeding_pattern (Animal_ID, Meal, Portion, Schedule_days, Schedule_time)
        VALUES (@AnimalId, @Meal, @Portion, @ScheduleDays, @ScheduleTime)
        """;

      command.Parameters.AddWithValue("@AnimalId", entity.AnimalId);
      command.Parameters.AddWithValue("@Meal", entity.Meal);
      command.Parameters.AddWithValue("@Portion", entity.Portion);
      command.Parameters.AddWithValue("@ScheduleDays", entity.ScheduleDays);
      command.Parameters.AddWithValue("@ScheduleTime", entity.ScheduleTime);

      await command.ExecuteNonQueryAsync();
    }

    public async Task UpdateAsync(FeedingPatternInfo entity){
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();

      command.CommandText = """
        UPDATE feeding_pattern
        SET Meal=@Meal, Portion=@Portion, Schedule_days=@ScheduleDays, Schedule_time=@ScheduleTime
        WHERE Animal_ID=@AnimalId
        """;

      command.Parameters.AddWithValue("@AnimalId", entity.AnimalId);
      command.Parameters.AddWithValue("@Meal", entity.Meal);
      command.Parameters.AddWithValue("@Portion", entity.Portion);
      command.Parameters.AddWithValue("@ScheduleDays", entity.ScheduleDays);
      command.Parameters.AddWithValue("@ScheduleTime", entity.ScheduleTime);

      await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteByIdAsync(int id){
      using var connection = _factory.CreateConnection();

      await connection.OpenAsync();

      await using var command = connection.CreateCommand();
      command.CommandText = """
        DELETE feeding_pattern WHERE Animal_ID = @id
        """;

      await using var reader = await command.ExecuteReaderAsync();
      command.Parameters.AddWithValue("@id", id);

      await command.ExecuteNonQueryAsync();
    }
  }
}
