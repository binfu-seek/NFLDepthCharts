using System;
using System.Collections.Generic;
using NFLDepthCharts.Models;

namespace NFLDepthCharts.Interfaces
{
    public interface IDepthChartService
    {
        void AddPlayerToChart(string position, PlayerModel player);
        void AddPlayerToChart(string position, PlayerModel player, int depth);

        PlayerModel RemovePlayerFromChart(string position, PlayerModel player);

        void GetBackups(string position, PlayerModel player);

        void GetFullDepthChart();
    }
}
