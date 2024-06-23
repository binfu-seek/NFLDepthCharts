using System;
using System.Collections.Generic;
using System.IO;
using NFLDepthCharts.Models;
using NFLDepthCharts.Services;
using Xunit;

namespace NFLDepthCharts.Tests
{
    public class NFLDepthChartsTests
    {
        public class AddPlayerToChartTests
        {
            [Fact]
            public void NullPlayerOrPosition_ShouldThrowException()
            {
                // Arrange
                var player = new PlayerModel { Number = 22, Name = "Christian McCaffrey" };
                var service = NFLDepthChartsTests.GetNewService();

                // Act
                var exception1 = Assert.Throws<Exception>(() => service.AddPlayerToChart(null, player));
                var exception2 = Assert.Throws<Exception>(() => service.AddPlayerToChart("QB", null));

                // Assert
                Assert.Equal("Position or player cannot be null.", exception1.Message);
                Assert.Equal("Position or player cannot be null.", exception2.Message);
            }

            [Fact]
            public void AddToNotExistingPosition_ShouldThrowException()
            {
                // Arrange
                var player = new PlayerModel { Number = 22, Name = "Christian McCaffrey" };
                var service = NFLDepthChartsTests.GetNewService();

                // Act
                var exception = Assert.Throws<Exception>(() => service.AddPlayerToChart("AA", player));

                // Assert
                Assert.Equal("Position 'AA' does not exist in the depth chart.", exception.Message);
            }

            [Fact]
            public void AddToPositionReachedMaxDepth_ShouldThrowException()
            {
                // Arrange
                var player1 = new PlayerModel { Number = 1, Name = "player1" };
                var player2 = new PlayerModel { Number = 2, Name = "player2" };
                var player3 = new PlayerModel { Number = 3, Name = "player3" };
                var player4 = new PlayerModel { Number = 4, Name = "player4" };
                var player5 = new PlayerModel { Number = 5, Name = "player5" };

                var realPlayer = new PlayerModel { Number = 22, Name = "Christian McCaffrey" };

                var service = NFLDepthChartsTests.GetNewService();

                service.AddPlayerToChart("QB", player1);
                service.AddPlayerToChart("QB", player2);
                service.AddPlayerToChart("QB", player3);
                service.AddPlayerToChart("QB", player4);
                service.AddPlayerToChart("QB", player5);

                // Act
                var exception = Assert.Throws<Exception>(() => service.AddPlayerToChart("QB", realPlayer));

                // Assert
                Assert.Equal("Cannot add player to position 'QB' because it has reached the maximum depth of 5.", exception.Message);
            }

            [Fact]
            public void AddDuplicatePlayer_ShouldThrowException()
            {
                // Arrange
                var player1 = new PlayerModel { Number = 22, Name = "Christian McCaffrey" };
                var player2 = new PlayerModel { Number = 22, Name = "Joe Wilson" };
                var service = NFLDepthChartsTests.GetNewService();
                service.AddPlayerToChart("QB", player1);

                // Act
                var exception = Assert.Throws<Exception>(() => service.AddPlayerToChart("QB", player2));

                // Assert
                Assert.Equal("Player (Number: 22, Name: Joe Wilson) already existed in position QB.", exception.Message);
            }

            [Theory]
            [InlineData(-1)]
            [InlineData(5)]
            public void GivenDepthOutOfRange_ShouldThrowException(int depth)
            {
                // Arrange
                var player = new PlayerModel { Number = 22, Name = "Christian McCaffrey" };
                var service = NFLDepthChartsTests.GetNewService();

                // Act
                var exception = Assert.Throws<Exception>(() => service.AddPlayerToChart("QB", player, depth));

                // Assert
                Assert.Equal("Depth must be between 0 and 4 (inclusive).", exception.Message);
            }

            [Theory]
            [InlineData(0)]
            [InlineData(1)]
            [InlineData(2)]
            [InlineData(3)]
            [InlineData(4)]
            public void DepthNotProvided_ShouldAddPlayerAtRear(int initialDepth)
            {
                // Arrange
                var service = NFLDepthChartsTests.GetNewService();
                var playerList = new List<PlayerModel> {
                    new PlayerModel { Number = 1, Name = "player1" },
                    new PlayerModel { Number = 2, Name = "player2" },
                    new PlayerModel { Number = 3, Name = "player3" },
                    new PlayerModel { Number = 4, Name = "player4" },
                };

                for (int i=0; i<initialDepth; i++)
                {
                    service.AddPlayerToChart("QB", playerList[i]);
                }

                var realPlayer = new PlayerModel { Number = 22, Name = "Christian McCaffrey" };

                // Act
                service.AddPlayerToChart("QB", realPlayer);
                var playerListAfter = service.GetFullDepthChartData()["QB"];

                // Assert
                Assert.Equal(realPlayer.Number, playerListAfter[initialDepth].Number);
            }

            [Theory]
            [InlineData(0)]
            [InlineData(1)]
            [InlineData(2)]
            [InlineData(3)]
            [InlineData(4)]
            public void DepthProvided_ShouldAddPlayerToIndex(int targetDepth)
            {
                // Arrange
                var service = NFLDepthChartsTests.GetNewService();
                var playerList = new List<PlayerModel> {
                    new PlayerModel { Number = 1, Name = "player1" },
                    new PlayerModel { Number = 2, Name = "player2" },
                    new PlayerModel { Number = 3, Name = "player3" },
                    new PlayerModel { Number = 4, Name = "player4" },
                };

                for (int i = 0; i < playerList.Count; i++)
                {
                    service.AddPlayerToChart("QB", playerList[i]);
                }

                var realPlayer = new PlayerModel { Number = 22, Name = "Christian McCaffrey" };

                // Act
                service.AddPlayerToChart("QB", realPlayer, targetDepth);
                var playerListAfter = service.GetFullDepthChartData()["QB"];

                // Assert
                Assert.Equal(realPlayer.Number, playerListAfter[targetDepth].Number);
            }

            [Theory]
            [InlineData(2)]
            [InlineData(3)]
            [InlineData(4)]
            public void DepthProvidedGreaterThanExistingPlayerCount_ShouldAddPlayerToIndex(int targetDepth)
            {
                // Arrange
                var service = NFLDepthChartsTests.GetNewService();
                var playerList = new List<PlayerModel> {
                    new PlayerModel { Number = 1, Name = "player1" },
                    new PlayerModel { Number = 2, Name = "player2" },
                };

                for (int i = 0; i < playerList.Count; i++)
                {
                    service.AddPlayerToChart("QB", playerList[i]);
                }

                var realPlayer = new PlayerModel { Number = 22, Name = "Christian McCaffrey" };

                // Act
                service.AddPlayerToChart("QB", realPlayer, targetDepth);
                var playerListAfter = service.GetFullDepthChartData()["QB"];

                // Assert
                Assert.Equal(realPlayer.Number, playerListAfter[2].Number);
            }
        }

        public class RemovePlayerFromChatTests
        {
            [Fact]
            public void NullPlayerOrPosition_ShouldThrowException()
            {
                // Arrange
                var player = new PlayerModel { Number = 22, Name = "Christian McCaffrey" };
                var service = NFLDepthChartsTests.GetNewService();

                // Act
                var exception1 = Assert.Throws<Exception>(() => service.RemovePlayerFromChart(null, player));
                var exception2 = Assert.Throws<Exception>(() => service.RemovePlayerFromChart("QB", null));

                // Assert
                Assert.Equal("Position or player cannot be null.", exception1.Message);
                Assert.Equal("Position or player cannot be null.", exception2.Message);
            }

            [Fact]
            public void PositionNotExist_ShouldThrowException()
            {
                // Arrange
                var player = new PlayerModel { Number = 22, Name = "Christian McCaffrey" };
                var service = NFLDepthChartsTests.GetNewService();

                // Act
                var exception = Assert.Throws<Exception>(() => service.RemovePlayerFromChart("AA", player));

                // Assert
                Assert.Equal("Position 'AA' does not exist in the depth chart.", exception.Message);
            }

            [Fact]
            public void RemoveNotExistingPlayer_ShouldReturnNull()
            {
                // Arrange
                var player = new PlayerModel { Number = 22, Name = "Christian McCaffrey" };
                var service = NFLDepthChartsTests.GetNewService();

                // Act
                var result = service.RemovePlayerFromChart("QB", player);

                // Assert
                Assert.Null(result);
            }

            [Theory]
            [InlineData(1)]
            [InlineData(2)]
            [InlineData(3)]
            [InlineData(4)]
            [InlineData(5)]
            public void RemovePlayerSuccess_ShouldReturnPlayerModel(int initialDepth)
            {
                // Arrange
                var player = new PlayerModel { Number = 1, Name = "player1" };
                var service = NFLDepthChartsTests.GetNewService(initialDepth);

                using (var sw = new StringWriter())
                {
                    Console.SetOut(sw);

                    // Act
                    var result = service.RemovePlayerFromChart("QB", player);
                    var playerListAfter = service.GetFullDepthChartData()["QB"];
                    var expectedOutput = "#1 - player1\n";

                    // Assert
                    Assert.Equal(expectedOutput, sw.ToString());
                    Assert.Equal(playerListAfter.Count, initialDepth - 1);
                }






                // Act
                //var result = service.RemovePlayerFromChart("QB", player);

                // Assert
                //Assert.Equal(result.Number, player.Number);

            }
        }

        public class GetBackupsTests
        {
            [Fact]
            public void NullPlayerOrPosition_ShouldThrowException()
            {
                // Similar like above. Do not repeat for now.
            }

            [Fact]
            public void PositionNotExist_ShouldThrowException()
            {
                // Similar like above. Do not repeat for now.
            }

            [Fact]
            public void NotExistingPlayer_ShouldPrintNoList()
            {
                // Arrange
                var player = new PlayerModel { Number = 22, Name = "Christian McCaffrey" };
                var service = NFLDepthChartsTests.GetNewService();

                using (var sw = new StringWriter())
                {
                    Console.SetOut(sw);

                    // Act
                    service.GetBackups("QB", player);

                    // Assert
                    var expectedOutput = "<NO LIST>\n";
                    Assert.Equal(expectedOutput, sw.ToString());
                }
            }

            [Fact]
            public void LastPlayerOfThePosition_ShouldPrintNoList()
            {
                // Arrange
                var player = new PlayerModel { Number = 22, Name = "Christian McCaffrey" };
                var service = NFLDepthChartsTests.GetNewService();
                service.AddPlayerToChart("QB", player);

                using (var sw = new StringWriter())
                {
                    Console.SetOut(sw);

                    // Act
                    service.GetBackups("QB", player);

                    // Assert
                    var expectedOutput = "<NO LIST>\n";
                    Assert.Equal(expectedOutput, sw.ToString());
                }
            }

            [Theory]
            [InlineData(0)]
            [InlineData(1)]
            [InlineData(2)]
            [InlineData(3)]
            public void PlayerHasBackups_ShouldPrintBackups(int playerIndex)
            {
                // Arrange
                //var positions = new List<string> { "QB" };
                //var service = new DepthChartService(positions);
                ////NFLDepthChartsTest.
                var playerList = new List<PlayerModel> {
                    new PlayerModel { Number = 1, Name = "player1" },
                    new PlayerModel { Number = 2, Name = "player2" },
                    new PlayerModel { Number = 3, Name = "player3" },
                    new PlayerModel { Number = 4, Name = "player4" },
                    new PlayerModel { Number = 5, Name = "player5" },
                };

                //for (int i = 0; i < playerList.Count; i++)
                //{
                //    service.AddPlayerToChart("QB", playerList[i]);
                //}

                var service = NFLDepthChartsTests.GetNewService(5);


                using (var sw = new StringWriter())
                {
                    Console.SetOut(sw);

                    // Act
                    service.GetBackups("QB", playerList[playerIndex]);
                    var expectedOutput = string.Empty;
                    //var expectedOutput = $"#{player2.Number} - {player2.Name}{Environment.NewLine}";
                    switch (playerIndex)
                    {
                        case 0:
                            expectedOutput = "#2 - player2\n#3 - player3\n#4 - player4\n#5 - player5\n";
                            break;
                        case 1:
                            expectedOutput = "#3 - player3\n#4 - player4\n#5 - player5\n";
                            break;
                        case 2:
                            expectedOutput = "#4 - player4\n#5 - player5\n";
                            break;
                        case 3:
                            expectedOutput = "#5 - player5\n";
                            break;
                        default:
                            break;
                    }

                    // Assert
                    Assert.Equal(expectedOutput, sw.ToString());
                }
            }
        }

        private static DepthChartService GetNewService(int playerCount = 0)
        {
            var service = new DepthChartService(new List<string> { "QB", "LWR" });
            var playerList = new List<PlayerModel> {
                    new PlayerModel { Number = 1, Name = "player1" },
                    new PlayerModel { Number = 2, Name = "player2" },
                    new PlayerModel { Number = 3, Name = "player3" },
                    new PlayerModel { Number = 4, Name = "player4" },
                    new PlayerModel { Number = 5, Name = "player5" },
                };

            if (playerCount > 5)
            {
                playerCount = 5;
            }

            for (int i = 0; i < playerCount; i++)
            {
                service.AddPlayerToChart("QB", playerList[i]);
            }

            return service;
        }
    }
}
