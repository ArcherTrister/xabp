// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.Cli.ProjectBuilding;
public class ProjectFile
{
    public string Path { get; }

    public string Name { get; }

    public int Depth { get; }

    public bool IsFolder { get; }

    private ProjectFile()
    {
    }

    public ProjectFile(
        string path,
        string name,
        int depth,
        bool isFolder)
    {
        Path = path;
        Name = name;
        Depth = depth;
        IsFolder = isFolder;
    }
}
