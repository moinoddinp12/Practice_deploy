namespace Online_Car_Marketplace.Services.Interfaces
{
    public interface IWishlistService
    {
        string AddToWishlist(int userId, int CarId);
        object GetWishlist(int userId);
        string RemoveFromWishlist(int userId, int CarId);
    }
}