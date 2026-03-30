class Barber {
  const Barber({
    required this.id,
    required this.name,
  });

  factory Barber.fromJson(Map<String, dynamic> json) {
    return Barber(
      id: json['id'] as String,
      name: json['name'] as String? ?? 'Unknown barber',
    );
  }

  final String id;
  final String name;
}

class ServiceItem {
  const ServiceItem({
    required this.id,
    required this.name,
    required this.durationMinutes,
    required this.price,
  });

  factory ServiceItem.fromJson(Map<String, dynamic> json) {
    return ServiceItem(
      id: json['id'] as String,
      name: json['name'] as String? ?? 'Service',
      durationMinutes: (json['durationMinutes'] as num?)?.toInt() ?? 30,
      price: (json['price'] as num?)?.toDouble() ?? 0,
    );
  }

  final String id;
  final String name;
  final int durationMinutes;
  final double price;
}

class TimeSlot {
  const TimeSlot({
    required this.start,
    required this.end,
    required this.isAvailable,
  });

  factory TimeSlot.fromJson(Map<String, dynamic> json) {
    return TimeSlot(
      start: DateTime.parse(json['start'] as String),
      end: DateTime.parse(json['end'] as String),
      isAvailable: json['isAvailable'] as bool? ?? true,
    );
  }

  final DateTime start;
  final DateTime end;
  final bool isAvailable;
}

class CustomerDraft {
  const CustomerDraft({
    this.id,
    this.firstname = '',
    this.lastname = '',
    this.phoneNumber = '',
    this.email = '',
    this.address = '',
  });

  CustomerDraft copyWith({
    String? id,
    String? firstname,
    String? lastname,
    String? phoneNumber,
    String? email,
    String? address,
  }) {
    return CustomerDraft(
      id: id ?? this.id,
      firstname: firstname ?? this.firstname,
      lastname: lastname ?? this.lastname,
      phoneNumber: phoneNumber ?? this.phoneNumber,
      email: email ?? this.email,
      address: address ?? this.address,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'firstname': firstname,
      'lastname': lastname,
      'phoneNumber': phoneNumber,
      'email': email,
      'address': address,
    };
  }

  final String? id;
  final String firstname;
  final String lastname;
  final String phoneNumber;
  final String email;
  final String address;
}

class BookingDraft {
  const BookingDraft({
    this.barber,
    this.service,
    this.date,
    this.slot,
    this.customer = const CustomerDraft(),
  });

  BookingDraft copyWith({
    Barber? barber,
    bool clearBarber = false,
    ServiceItem? service,
    bool clearService = false,
    DateTime? date,
    bool clearDate = false,
    TimeSlot? slot,
    bool clearSlot = false,
    CustomerDraft? customer,
  }) {
    return BookingDraft(
      barber: clearBarber ? null : (barber ?? this.barber),
      service: clearService ? null : (service ?? this.service),
      date: clearDate ? null : (date ?? this.date),
      slot: clearSlot ? null : (slot ?? this.slot),
      customer: customer ?? this.customer,
    );
  }

  final Barber? barber;
  final ServiceItem? service;
  final DateTime? date;
  final TimeSlot? slot;
  final CustomerDraft customer;
}
