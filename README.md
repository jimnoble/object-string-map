# Object-String Map

Use a text map to turn objects into strings and back again.

## Use

- Create a class for your data. A flat POCO with simple types such as int, string, Guid and DateTime:
```
public class AccountKey
{
    int AccountId { get; set; }
}
```
- Create a map that describes how to turn your data to and from a string. The syntax is the same as C# 6.0's new string interpolation feature:
```
/accounts/{AccountId}.txt
```
