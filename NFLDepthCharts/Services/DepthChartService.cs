using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NFLDepthCharts.Interfaces;
using NFLDepthCharts.Models;

namespace NFLDepthCharts.Services
{
    public class DepthChartService : IDepthChartService
    {
        private Dictionary<string, List<PlayerModel>> _depthChart;
        private int _maxDepth;

        private const int DEFAULT_MAX_DEPTH = 5;

        /// <summary>
        /// Initializes a new instance of the <see cref="DepthChartService"/> class.
        /// </summary>
        /// <param name="positions">The list of positions to initialize in the depth chart.</param>
        /// <param name="maxDepth">The maximum depth for each position. Defaults to 5.</param>
        /// <exception cref="Exception">Thrown when positions list is null or empty, contains invalid strings, or maxDepth is less than or equal to 0.</exception>
        public DepthChartService(List<string> positions, int maxDepth = DEFAULT_MAX_DEPTH)
        {
            if (positions == null || positions.Count == 0)
            {
                throw new Exception("Position list cannot be null or empty!");
            }

            if (positions.Any(p => String.IsNullOrEmpty(p)))
            {
                throw new Exception("Position list contains invalid string!");
            }

            if (maxDepth <= 0)
            {
                throw new Exception("Max depth cannot be 0 or negative!");
            }

            _depthChart = new Dictionary<string, List<PlayerModel>>();
            positions.ForEach(p => _depthChart.Add(p.Trim().ToUpper(), new List<PlayerModel>()));
            _maxDepth = maxDepth;
        }

        /// <summary>
        /// Adds a player to the depth chart at the specified position, appending to the end of the list.
        /// </summary>
        /// <param name="position">The position to which the player should be added.</param>
        /// <param name="player">The player to add.</param>
        /// <exception cref="Exception">Thrown when the position does not exist in the depth chart, the player is null, or the position has reached its maximum depth.</exception>
        public void AddPlayerToChart(string position, PlayerModel player)
        {
            AddPlayer(position, player, null);
        }


        /// <summary>
        /// Adds a player to the depth chart at the specified position and depth.
        /// </summary>
        /// <param name="position">The position to which the player should be added.</param>
        /// <param name="player">The player to add.</param>
        /// <param name="depth">The depth at which the player should be added.</param>
        /// <exception cref="Exception">Thrown when the position does not exist in the depth chart, the player is null, the position has reached its maximum depth, or the specified depth is invalid.</exception>
        public void AddPlayerToChart(string position, PlayerModel player, int depth)
        {
            AddPlayer(position, player, depth);
        }

        /// <summary>
        /// Removes a player from the depth chart at the specified position.
        /// </summary>
        /// <param name="position">The position from which the player should be removed.</param>
        /// <param name="player">The player to remove.</param>
        /// <returns>The removed player if found; otherwise, null.</returns>
        /// <exception cref="Exception">Thrown when the position does not exist in the depth chart.</exception>
        public PlayerModel RemovePlayerFromChart(string position, PlayerModel player)
        {
            if (player == null || string.IsNullOrEmpty(position))
            {
                throw new Exception("Position or player cannot be null.");
            }

            position = position.Trim().ToUpper();
            if (!_depthChart.ContainsKey(position))
            {
                throw new Exception($"Position '{position}' does not exist in the depth chart.");
            }

            var positionPlayers = _depthChart[position.Trim().ToUpper()];
            var playerInList = positionPlayers.FirstOrDefault(p => p.Number == player.Number);
            if (positionPlayers.Remove(playerInList))
            {
                Console.WriteLine($"#{playerInList.Number} - {playerInList.Name}");
                return playerInList;
            }
            return null;
        }

        /// <summary>
        /// Retrieves and prints the backups for a given player at a specific position.
        /// </summary>
        /// <param name="position">The position of the player.</param>
        /// <param name="player">The player for whom backups are to be retrieved.</param>
        public void GetBackups(string position, PlayerModel player)
        {
            if (player == null || string.IsNullOrEmpty(position))
            {
                throw new Exception("Position or player cannot be null.");
            }

            position = position.Trim().ToUpper();
            if (!_depthChart.ContainsKey(position))
            {
                throw new Exception($"Position '{position}' does not exist in the depth chart.");
            }

            var positionPlayers = _depthChart[position.Trim().ToUpper()];
            var index = positionPlayers.FindIndex(pp => pp.Number == player.Number);
            if (index == -1 || index == positionPlayers.Count - 1)
            {
                Console.WriteLine("<NO LIST>");
                return;
            }

            //var backupPlayers = new List<PlayerModel>();
            //backupPlayers = positionPlayers.GetRange(index + 1, positionPlayers.Count - index - 1);
            var backupPlayers = positionPlayers.Skip(index + 1).ToList();

            if (!backupPlayers.Any())
            {
                Console.WriteLine("<NO LIST>");
                return;
            }

            var sb = new StringBuilder();
            foreach (PlayerModel p in backupPlayers)
            {
                sb.AppendLine($"#{p.Number} - {p.Name}");
            }

            Console.Write(sb.ToString());
        }

        /// <summary>
        /// Prints the entire depth chart, listing players at each position along with their depth.
        /// </summary>
        public void GetFullDepthChart()
        {
            var sb = new StringBuilder();

            foreach (KeyValuePair<string, List<PlayerModel>> pos in _depthChart)
            {
                sb.Append(pos.Key + " - ");
                for (int i = 0; i < pos.Value.Count; i++)
                {
                    var player = pos.Value[i];
                    sb.Append($"(#{player.Number}, {player.Name})");
                    if (i < pos.Value.Count - 1)
                    {
                        sb.Append(", ");
                    }
                }
                sb.Append(Environment.NewLine);
            }

            Console.WriteLine(sb.ToString());
        }

        // Not required in the original requirement.
        // It provides convenience for testing.
        public Dictionary<string, List<PlayerModel>> GetFullDepthChartData()
        {
            return _depthChart.ToDictionary(
                entry => entry.Key,
                entry => entry.Value.Select(player => new PlayerModel
                {
                    Number = player.Number,
                    Name = player.Name
                }).ToList());
        }

        // Private methods
        private void AddPlayer(string position, PlayerModel player, int? depth)
        {
            if (player == null || string.IsNullOrEmpty(position))
            {
                throw new Exception("Position or player cannot be null.");
            }

            position = position.Trim().ToUpper();
            if (!_depthChart.ContainsKey(position))
            {
                throw new Exception($"Position '{position}' does not exist in the depth chart.");
            }
            var positionPlayers = _depthChart[position];
            if (positionPlayers.Count == _maxDepth)
            {
                throw new Exception($"Cannot add player to position '{position}' because it has reached the maximum depth of {_maxDepth}.");
            }

            // If player already exists in the position
            if (positionPlayers.Any(pp => pp.Number == player.Number))
            {
                throw new Exception($"Player (Number: { player.Number }, Name: {player.Name}) already existed in position {position}.");
            }

            if (depth.HasValue)
            {
                if (depth < 0 || depth >= _maxDepth)
                {
                    throw new Exception($"Depth must be between 0 and {_maxDepth - 1} (inclusive).");
                }
                else if (depth.Value > positionPlayers.Count)
                {
                    positionPlayers.Add(player);
                } else
                {
                    positionPlayers.Insert(depth.Value, player);
                }
            } else
            {
                positionPlayers.Add(player);
            }
        }
    }
}
