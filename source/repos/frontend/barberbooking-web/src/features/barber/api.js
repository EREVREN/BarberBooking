import { http } from "../../shared/api/http";

export const getBarbers = async () => {
    const response = await http.get("/api/barbers");
    return response.data;
};