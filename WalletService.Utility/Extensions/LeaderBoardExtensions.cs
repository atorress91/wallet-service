using WalletService.Models.Constants;
using WalletService.Models.DTO.LeaderBoardDto;

namespace WalletService.Utility.Extensions;

public static class LeaderBoardExtensions
{

    public static List<LeaderBoardModel4> OrderModel4(this IEnumerable<LeaderBoardModel4> users)
    {
        var orderedUsers = users
            .OrderByDescending(u => u.Amount)
            .ThenBy(u => u.UserCreatedAt)
            .ToList();

        var queue         = new Queue<LeaderBoardModel4>();
        var childrenCount = 0;

        if (orderedUsers.Count > 0)
        {
            var firstUser = orderedUsers[0];
            firstUser.PositionX = 0; 
            firstUser.PositionY = 0; 
            firstUser.Level     = 0;
            queue.Enqueue(firstUser);
        }

        for (var i = 1; i < orderedUsers.Count; i++)
        {
            var user   = orderedUsers[i];
            var father = queue.Peek();

            if (childrenCount < Constants.ChildrenLimitModel4)
            {
                childrenCount++;
                user.FatherModel4 = father.AffiliateId;
                user.PositionX    = childrenCount;
                user.PositionY    = father.Level + 1;
                user.Level        = father.Level + 1;
            }
            else 
            {
                queue.Dequeue();
                father            = queue.Peek(); 
                childrenCount     = 1;
                user.FatherModel4 = father.AffiliateId; 
                user.PositionX    = childrenCount; 
                user.PositionY    = father.Level + 1;
                user.Level        = father.Level + 1;
            }

            queue.Enqueue(user); 
        }

        return orderedUsers;
    }
    
    public static List<LeaderBoardModel5> OrderModel5(this IEnumerable<LeaderBoardModel5> users)
    {
        var orderedUsers = users
            .OrderByDescending(u => u.Amount)
            .ThenBy(u => u.UserCreatedAt)
            .ToList();

        var queue         = new Queue<LeaderBoardModel5>();
        var childrenCount = 0;

        if (orderedUsers.Count > 0)
        {
            var firstUser = orderedUsers[0];
            firstUser.PositionX = 0; 
            firstUser.PositionY = 0; 
            firstUser.Level     = 0;
            queue.Enqueue(firstUser);
        }

        for (var i = 1; i < orderedUsers.Count; i++)
        {
            var user   = orderedUsers[i];
            var father = queue.Peek();

            if (childrenCount < Constants.ChildrenLimitModel5)
            {
                childrenCount++;
                user.FatherModel5 = father.AffiliateId;
                user.PositionX    = childrenCount;
                user.PositionY    = father.Level + 1;
                user.Level        = father.Level + 1;
            }
            else 
            {
                queue.Dequeue();
                father            = queue.Peek(); 
                childrenCount     = 1;
                user.FatherModel5 = father.AffiliateId; 
                user.PositionX    = childrenCount; 
                user.PositionY    = father.Level + 1;
                user.Level        = father.Level + 1;
            }

            queue.Enqueue(user); 
        }

        return orderedUsers;
    }
    public static List<LeaderBoardModel6> OrderModel6(this IEnumerable<LeaderBoardModel6> users)
    {
        var orderedUsers = users
            .OrderByDescending(u => u.Amount)
            .ThenBy(u => u.UserCreatedAt)
            .ToList();

        var queue         = new Queue<LeaderBoardModel6>();
        var childrenCount = 0;

        if (orderedUsers.Count > 0)
        {
            var firstUser = orderedUsers[0];
            firstUser.PositionX = 0; 
            firstUser.PositionY = 0; 
            firstUser.Level     = 0;
            queue.Enqueue(firstUser);
        }

        for (var i = 1; i < orderedUsers.Count; i++)
        {
            var user   = orderedUsers[i];
            var father = queue.Peek();

            if (childrenCount < Constants.ChildrenLimitModel6)
            {
                childrenCount++;
                user.FatherModel6 = father.AffiliateId;
                user.PositionX    = childrenCount;
                user.PositionY    = father.Level + 1;
                user.Level        = father.Level + 1;
            }
            else 
            {
                queue.Dequeue();
                father            = queue.Peek(); 
                childrenCount     = 1;
                user.FatherModel6 = father.AffiliateId; 
                user.PositionX    = childrenCount; 
                user.PositionY    = father.Level + 1;
                user.Level        = father.Level + 1;
            }

            queue.Enqueue(user); 
        }

        return orderedUsers;
    }

}