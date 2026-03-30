import {http} from "@/shared/api/http"; 

export function fetchAdminBookings({barberId, from, to}) {
    return http.get('/admin/appointments', {
        params: { barberId, from, to }
            });
           }   