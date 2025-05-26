using Kaiju.Command;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaiju
{
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

        private DatabaseManager()
        {
            string dataFolderPath = Path.Combine(AppContext.BaseDirectory, "Data");
            if (!Directory.Exists(dataFolderPath))
            {
                Directory.CreateDirectory(dataFolderPath);
            }
            dbPath = Path.Combine(dataFolderPath, "KaijuPlayerData.db");
        }

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

        public void PrintPlayerStats(string playerName)
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
                Debug.WriteLine($"Player: {reader["PlayerName"]}");
                Debug.WriteLine($"Wins: {reader["Wins"]}, Losses: {reader["Losses"]}, Draws: {reader["Draws"]}");
                Debug.WriteLine($"Games Played: {reader["GamesPlayed"]}");
                Debug.WriteLine($"Win/Loss Ratio: {reader["WinLossRatio"]}");
                Debug.WriteLine($"Favorite Character: {reader["FavoriteCharacter"]}");
            }
            else
            {
                Debug.WriteLine("Player not found.");
            }
        }

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
    }
}
