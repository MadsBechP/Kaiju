using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Kaiju
{
    /// <summary>
    /// A singleton class that controls and manages the database
    /// Handles initialization, Match results and player stats retrieval
    /// </summary>
    public class DatabaseManager
    {
        private static DatabaseManager instance;

        public static DatabaseManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DatabaseManager();
                }
                return instance;
            }
        }

        private readonly string dbPath;

        /// <summary>
        /// Contructor
        /// Sets the database path and ensures the database directory exists
        /// </summary>
        private DatabaseManager()
        {
            string dataFolderPath = Path.Combine(AppContext.BaseDirectory, "Data");
            if (!Directory.Exists(dataFolderPath))
            {
                Directory.CreateDirectory(dataFolderPath);
            }
            dbPath = Path.Combine(dataFolderPath, "KaijuPlayerData.db");
        }

        /// <summary>
        /// Initializes the database and creates the Player table if it does not exist
        /// </summary>
        public void Initialize()
        {
            using var connection = new SqliteConnection($"Data Source={dbPath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Player (
                PlayerID               INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                PlayerName             TEXT    NOT NULL UNIQUE,
                Wins                   INTEGER DEFAULT 0,
                Losses                 INTEGER DEFAULT 0,
                Draws                  INTEGER DEFAULT 0,
                [KO's]                 INTEGER DEFAULT 0,
                [KO'd]                 INTEGER DEFAULT 0,
                [TimesPicked Godzilla] INTEGER DEFAULT 0,
                [TimesPicked Gigan]    INTEGER DEFAULT 0,

                GamesPlayed            INTEGER GENERATED ALWAYS AS (Wins + Losses + Draws) VIRTUAL,
                WinLossRatio           REAL GENERATED ALWAYS AS (
                                          CASE 
                                            WHEN Losses = 0 THEN Wins 
                                            ELSE CAST(Wins AS REAL) / Losses 
                                          END
                                        ) VIRTUAL,
                FavoriteCharacter      TEXT GENERATED ALWAYS AS (
                                          CASE 
                                            WHEN [TimesPicked Godzilla] > [TimesPicked Gigan] THEN 'Godzilla'
                                            WHEN [TimesPicked Gigan] > [TimesPicked Godzilla] THEN 'Gigan'
                                            WHEN [TimesPicked Godzilla] = [TimesPicked Gigan] AND [TimesPicked Godzilla] > 0 THEN 'Tie'
                                            ELSE NULL
                                          END
                                        ) VIRTUAL
            );";
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Writes the player stats to the debug (Needs update to write to screen
        /// </summary>
        /// <param name="playerName">The name of the player in the database</param>
        public PlayerStats PrintPlayerStats(string playerName)
        {
            using var connection = new SqliteConnection($"Data Source={dbPath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
            SELECT 
                PlayerName, 
                Wins, 
                Losses, 
                Draws, 
                GamesPlayed, 
                WinLossRatio, 
                FavoriteCharacter 
            FROM Player 
            WHERE PlayerName = $name;";
            command.Parameters.AddWithValue("$name", playerName);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new PlayerStats
                {
                    PlayerName = reader.GetString(0),
                    Wins = reader.GetInt32(1),
                    Losses = reader.GetInt32(2),
                    Draws = reader.GetInt32(3),
                    GamesPlayed = reader.GetInt32(4),
                    WinLossRatio = reader.IsDBNull(5) ? 0.0 : reader.GetDouble(5),
                    FavoriteCharacter = reader.IsDBNull(6) ? "None" : reader.GetString(6)
                };
            }

            return null;
        }

        /// <summary>
        /// Records the results of the match in the database
        /// </summary>
        /// <param name="playerName">The player that played the match</param>
        /// <param name="won">If they won</param>
        /// <param name="drew">if they lost</param>
        /// <param name="characterPlayed">The character the player played</param>
        /// <exception cref="ArgumentException"></exception>
        public void RecordMatchResult(string playerName, bool won, bool drew, string characterPlayed)
        {
            using var connection = new SqliteConnection($"Data Source={dbPath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
            INSERT INTO Player (PlayerName, Wins, Losses, Draws, [TimesPicked Godzilla], [TimesPicked Gigan])
            VALUES ($name, 0, 0, 0, 0, 0)
            ON CONFLICT(PlayerName) DO NOTHING;";
            command.Parameters.AddWithValue("$name", playerName);
            command.ExecuteNonQuery();

            if (characterPlayed == "Godzilla")
            {
                command.CommandText = @"
            UPDATE Player
            SET Wins = Wins + $win,
                Losses = Losses + $loss,
                Draws = Draws + $draw,
                [TimesPicked Godzilla] = [TimesPicked Godzilla] + 1
            WHERE PlayerName = $name;";
            }
            else if (characterPlayed == "Gigan")
            {
                command.CommandText = @"
            UPDATE Player
            SET Wins = Wins + $win,
                Losses = Losses + $loss,
                Draws = Draws + $draw,
                [TimesPicked Gigan] = [TimesPicked Gigan] + 1
            WHERE PlayerName = $name;";
            }
            else
            {
                throw new ArgumentException("Unknown character: " + characterPlayed);
            }

            command.Parameters.Clear();
            command.Parameters.AddWithValue("$name", playerName);
            command.Parameters.AddWithValue("$win", won ? 1 : 0);
            command.Parameters.AddWithValue("$loss", (!won && !drew) ? 1 : 0);
            command.Parameters.AddWithValue("$draw", drew ? 1 : 0);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Adds a new player to the database if it does not exist
        /// </summary>
        /// <param name="playerName">The name of the new database</param>
        public void AddNewProfile(string playerName)
        {
            using var connection = new SqliteConnection($"Data Source={dbPath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Player (PlayerName, Wins, Losses, Draws, [KO's], [KO'd], [TimesPicked Godzilla], [TimesPicked Gigan])
                VALUES ($name, 0, 0, 0, 0, 0, 0, 0)
                ON CONFLICT(PlayerName) DO NOTHING;";
            command.Parameters.AddWithValue("$name", playerName);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected == 0)
            {
                Debug.WriteLine($"Profile {playerName} already exists");
            }
            else
            {
                Debug.WriteLine($"New profile {playerName} added successfully");
            }
        }

        public List<string> ListAllPlayerNames()
        {
            var playerNames = new List<string>();

            using var connection = new SqliteConnection($"Data Source={dbPath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"SELECT PlayerName FROM Player ORDER BY PlayerName ASC;";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                playerNames.Add(reader.GetString(0));
            }

            return playerNames;
        }
    }

    public class PlayerStats
    {
        public string PlayerName;
        public int Wins;
        public int Losses;
        public int Draws;
        public int GamesPlayed;
        public double WinLossRatio;
        public string FavoriteCharacter;
    }
}
