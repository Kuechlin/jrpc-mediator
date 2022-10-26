namespace Example.Contract.Models;

public class TodoModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public TodoState State { get; set; } = TodoState.New;
}