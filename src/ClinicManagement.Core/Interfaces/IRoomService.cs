using ClinicManagement.Core.Entities;

namespace ClinicManagement.Core.Interfaces;

public interface IRoomService
{
    Task<(bool Success, string? ErrorCode, Room? Room)> CreateRoomAsync(
        string roomNumber,
        string roomName,
        string roomType,
        int capacity = 1,
        string? equipment = null,
        string? description = null,
        decimal? hourlyRate = null);

    Task<(bool Success, string? ErrorCode, Room? Room)> UpdateRoomAsync(
        Guid roomId,
        string roomNumber,
        string roomName,
        string roomType,
        int capacity,
        string? equipment,
        string? description,
        decimal? hourlyRate,
        bool isActive);

    Task<(bool Success, string? ErrorCode, RoomBooking? Booking)> BookRoomAsync(
        Guid roomId,
        DateTime startTime,
        DateTime endTime,
        string purpose,
        Guid? appointmentId = null);

    Task<bool> IsRoomAvailableAsync(Guid roomId, DateTime startTime, DateTime endTime, Guid? excludeBookingId = null);
    
    Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime startTime, DateTime endTime, string? roomType = null);
    
    Task<Room?> GetRoomAsync(Guid roomId);
    
    Task<IEnumerable<Room>> GetActiveRoomsAsync();
    
    Task<IEnumerable<RoomBooking>> GetRoomBookingsAsync(Guid roomId, DateTime date);
}