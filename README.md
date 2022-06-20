# Build Versioner

A build versioner that uses perforce to stamp the latest commit along with major/minor version.

### Basic Usage

![Build Versioner in Unity](/Documentation~/window.png)

1) Window > Build Versioner
2) Add your workspace name and test with `Get Changelist Number`
  2a) If an error mentions your username, make sure to set it with `Set Username`
  2b) If an error mentions session expired, open and login to the perforce client
3) Set the Major and Minor versions

*Note: These changes are local to your machine and will not be tracked in source control.* 

### Usage in Code
The only public API with this package is through `BuildVersioner.BVInfo`, so use this class when you want to access the BV data. An example of a script using BVInfo might look as simple as:

```cs
public class Example : MonoBehaviour
{
    private void Start()
    {
        var text = GetComponent<Text>();
        text.text = BuildVersioner.BVInfo.GetVersionFormatted();
    }
}
```

Which during Play would look like this:

![Example1](/Documentation~/example1.png)