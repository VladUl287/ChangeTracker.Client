using System.Reflection;
using System.Reflection.Emit;
using Tracker.Core.Extensions;

namespace Tracker.Core.Tests;

public class AssemblyExtensionsTests
{
    [Fact]
    public void Default_Executing_Assembly()
    {
        // Arrange
        var assembly = Assembly.GetExecutingAssembly();

        // Act
        var writeTime = assembly.GetAssemblyWriteTime();
        var lastWriteTimeUtc = File.GetLastWriteTimeUtc(assembly.Location);

        // Assert
        Assert.Equal(writeTime, lastWriteTimeUtc);
    }

    [Fact]
    public void Null_Assembly()
    {
        // Arrange
        var assembly = Assembly.GetExecutingAssembly();

        // Act
        var writeTime = assembly.GetAssemblyWriteTime();
        var lastWriteTimeUtc = File.GetLastWriteTimeUtc(assembly.Location);

        // Assert
        Assert.Equal(writeTime, lastWriteTimeUtc);
    }

    [Fact]
    public void Dynamic_Assembly()
    {
        // Arrange
        var assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("NullLocationAssembly"), AssemblyBuilderAccess.RunAndCollect);

        // Act
        var getWriteTime = () => { assembly.GetAssemblyWriteTime(); };

        // Assert
        Assert.Throws<ArgumentException>(getWriteTime);
    }

    [Fact]
    public void Dynamic_Assembly_W()
    {
        // Arrange
        var assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("NullLocationAssembly"), AssemblyBuilderAccess.RunAndCollect);

        // Act
        var getWriteTime = () => { assembly.GetAssemblyWriteTime(); };

        // Assert
        Assert.Throws<ArgumentException>(getWriteTime);
    }
}
