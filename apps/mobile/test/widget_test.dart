import 'package:flutter_test/flutter_test.dart';

import 'package:barberbooking_mobile/main.dart';
import 'package:barberbooking_mobile/src/booking_api.dart';
import 'package:barberbooking_mobile/src/models.dart';

class _FakeApi extends BarberBookingApi {
  @override
  Future<List<Barber>> fetchBarbers() async {
    return const [
      Barber(id: '1', name: 'Mason'),
      Barber(id: '2', name: 'Leo'),
    ];
  }
}

void main() {
  testWidgets('renders booking shell', (tester) async {
    await tester.pumpWidget(BarberBookingApp(api: _FakeApi()));
    await tester.pumpAndSettle();

    expect(find.text('Welcome to BarberBooking'), findsOneWidget);
    expect(find.text('Book Now'), findsOneWidget);
    expect(find.text('AI Booking'), findsOneWidget);
  });
}
