public interface ISaveable
{
    string SaveId { get; }          // e.g., "Player.Transform" / "Player.Stats"
    string CaptureJson();           // returns JSON for this object
    void RestoreFromJson(string json);
}
