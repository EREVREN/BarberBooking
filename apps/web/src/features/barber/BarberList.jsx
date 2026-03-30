import barberImg1 from '@/assets/img/barber-1.jpg';
import barberImg2 from '@/assets/img/barber-2.jpg';
import barberImg3 from '@/assets/img/barber-3.jpg';
import { useBarbers } from './hooks/useBarbers';

export function BarberList() {
    const { data, isLoading, error } = useBarbers();
    
    // Array of barber images
    const barberImages = [barberImg1, barberImg2, barberImg3];

    if (isLoading) return <p>Loading...</p>;
    if (error) return <p>Error loading barbers</p>;

    return (
        data && data.length > 0 ? (
            <div className="grid md:grid-cols-3 gap-6 hover:shadow-lg transition">
                {data.map((barber, index) => (
                  <div
                      key={barber.id}
                      className="bg-white p-6 rounded-xl shadow hover:shadow-lg transition" 
                      >
                    <div className="w-96 h-84 sm:w-36 sm:h-36 md:w-64 md:h-64 mb-4 flex-shrink-0">
                        <img 
                          src={barberImages[index % barberImages.length]} 
                          alt={barber.name} 
                          className="rounded-2xl object-cover w-full h-full"
                        />
                      </div>
                      <h3 className="text-xl font-semibold">{barber.name}</h3>
                    </div>
                ))}
            </div>
        ) : (
            <p>No barbers found.</p>
        )
    );
}

//"bg-white p-6 rounded-xl shadow hover:shadow-lg transition text-center flex flex-col items-center w-full h-full"
                    