# NFLDepthCharts

## Overview

This solution includes two projects:
- NFLDepthCharts

    This is the main project that implementes all the functionalities mentioned in the requirement. 
    It is a simple console application built with .Net Core 3.1

- NFLDepthCharts.Tests

    This is a testing project contains comprehensive test cases for all major functions.

## Considerations
- IDepthChartService Interface

    This interface defines a set of functions which are implemented in `DepthChartService` class. This gives the client an option to use Dependency Injection to only depend on the abstracted contract rather than the specific service class.

- Using XML documentation annotations

    These annotations make the code more readable and maintainable, providing useful context and information to developers using or modifying the class. 

- Max depth

    Added a max depth for the chart and default to 5 if not provided. This prevent adding too many players in one position by mistake.

- Duplication check

    When adding a player to a position, check if the player is already on the list. This prevents the same player appears on the same position more than once.

- Dynamic position list for different games

    The list of positions such as ["QB", "LWR"] are dynamically provided when instantiating the service. Therefore for  games other than NFL, this code can be reused by providing a different set of positions.

- If there are multiple teams

    One instance of DepthChartService stores data for one team. So if there are multiple teams, multiple instances can be created separately. 

## Data structure
Dictionary is used to store the chart. `_depthChart` in DepthChartService class holds everything about the chart.
> Dictionary<string, List\<PlayerModel>> _depthChart;
- The string key is the position name, such as "QB" or "LWR"
- The value is a List<PlayerModel>, which represents all the players under this position.
- The `PlayerModel` contains two fields: Number (int) and Name (string) to uniquely identify each player.

## How to build and run
- Clone the repo to a local folder
- Open `NFLDepthCharts.sln` file using VisualStudio
- Build the solution
- In Program.cs under the main project, there are two functions sample the usage of the class/functions, and they have been called in the main() function. When the application runs, they prints out information in the terminal.

## Improvements to be done
- Persist data in database

    As mentioned in the requirement, in-memery data structure is acceptable for this task, so I use a dictionary directly in the DepthChartService to store the chart data. However, in real projects this data needs to be stored in a database, so the DBContext and repository need to be implemented and the service class here will do CRUD operations through them.

