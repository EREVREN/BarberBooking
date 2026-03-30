import { useQuery } from "@tanstack/react-query";
import { fetchAdminBookings } from "@/shared/api/adminBookings";

export function useAdminBookings(params) {
  return useQuery({
    queryKey: ["admin-bookings", params],
    queryFn: () => fetchAdminBookings(params),
    keepPreviousData: true,
  });
}