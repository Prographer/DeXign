namespace DeXign.Models
{
    public interface IViewModel<TModel>
        where TModel : IModel
    {
        TModel Model { get; set; }
    }
}
