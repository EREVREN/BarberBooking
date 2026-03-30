import 'package:flutter/material.dart';

import 'booking_api.dart';
import 'home_page.dart';
import 'models.dart';

class BookingPage extends StatefulWidget {
  const BookingPage({super.key, this.api});

  final BarberBookingApi? api;

  @override
  State<BookingPage> createState() => _BookingPageState();
}

class _BookingPageState extends State<BookingPage> {
  late final BarberBookingApi _api;
  final _formKey = GlobalKey<FormState>();
  final _firstNameController = TextEditingController();
  final _lastNameController = TextEditingController();
  final _phoneController = TextEditingController();
  final _emailController = TextEditingController();
  final _addressController = TextEditingController();

  BookingDraft _booking = const BookingDraft();
  int _step = 0;
  bool _loading = false;
  String? _error;
  String? _successMessage;
  List<Barber> _barbers = const [];
  List<ServiceItem> _services = const [];
  List<TimeSlot> _slots = const [];

  @override
  void initState() {
    super.initState();
    _api = widget.api ?? BarberBookingApi();
    _loadBarbers();
  }

  @override
  void dispose() {
    _firstNameController.dispose();
    _lastNameController.dispose();
    _phoneController.dispose();
    _emailController.dispose();
    _addressController.dispose();
    super.dispose();
  }

  Future<void> _loadBarbers() async {
    await _runLoad(
      task: () async => _barbers = await _api.fetchBarbers(),
      errorMessage: 'Failed to load barbers.',
    );
  }

  Future<void> _loadServices() async {
    final barber = _booking.barber;
    if (barber == null) {
      return;
    }
    await _runLoad(
      task: () async => _services = await _api.fetchServices(barber.id),
      errorMessage: 'Failed to load services.',
    );
  }

  Future<void> _loadSlots() async {
    final barber = _booking.barber;
    final service = _booking.service;
    final date = _booking.date;
    if (barber == null || service == null || date == null) {
      return;
    }
    await _runLoad(
      task: () async {
        _slots = await _api.fetchAvailability(
          barberId: barber.id,
          date: date,
          serviceDurationMinutes: service.durationMinutes,
        );
      },
      errorMessage: 'Failed to load availability.',
    );
  }

  Future<void> _runLoad({
    required Future<void> Function() task,
    required String errorMessage,
  }) async {
    setState(() {
      _loading = true;
      _error = null;
    });
    try {
      await task();
    } on ApiException catch (error) {
      _error = error.message.isEmpty ? errorMessage : error.message;
    } catch (_) {
      _error = errorMessage;
    } finally {
      if (mounted) {
        setState(() {
          _loading = false;
        });
      }
    }
  }

  void _selectBarber(Barber barber) {
    setState(() {
      _booking = _booking.copyWith(
        barber: barber,
        clearService: true,
        clearDate: true,
        clearSlot: true,
      );
      _services = const [];
      _slots = const [];
      _step = 1;
      _error = null;
      _successMessage = null;
    });
    _loadServices();
  }

  void _selectService(ServiceItem service) {
    setState(() {
      _booking = _booking.copyWith(
        service: service,
        clearDate: true,
        clearSlot: true,
      );
      _slots = const [];
      _step = 2;
      _error = null;
    });
  }

  Future<void> _pickDate() async {
    final now = DateTime.now();
    final selected = await showDatePicker(
      context: context,
      initialDate: _booking.date ?? now,
      firstDate: DateTime(now.year, now.month, now.day),
      lastDate: DateTime(now.year + 1),
    );
    if (selected == null) {
      return;
    }
    setState(() {
      _booking = _booking.copyWith(
        date: DateTime(selected.year, selected.month, selected.day),
        clearSlot: true,
      );
      _slots = const [];
      _step = 3;
      _error = null;
    });
    await _loadSlots();
  }

  void _selectSlot(TimeSlot slot) {
    setState(() {
      _booking = _booking.copyWith(slot: slot);
      _step = 4;
      _error = null;
    });
    _syncCustomerForm();
  }

  void _syncCustomerForm() {
    final customer = _booking.customer;
    _firstNameController.text = customer.firstname;
    _lastNameController.text = customer.lastname;
    _phoneController.text = customer.phoneNumber;
    _emailController.text = customer.email;
    _addressController.text = customer.address;
  }

  void _saveCustomerStep() {
    if (!_formKey.currentState!.validate()) {
      return;
    }
    setState(() {
      _booking = _booking.copyWith(
        customer: _booking.customer.copyWith(
          firstname: _firstNameController.text.trim(),
          lastname: _lastNameController.text.trim(),
          phoneNumber: _phoneController.text.trim(),
          email: _emailController.text.trim(),
          address: _addressController.text.trim(),
        ),
      );
      _step = 5;
      _error = null;
    });
  }

  Future<void> _confirmBooking() async {
    final barber = _booking.barber;
    final service = _booking.service;
    final slot = _booking.slot;
    if (barber == null || service == null || slot == null) {
      return;
    }

    setState(() {
      _loading = true;
      _error = null;
      _successMessage = null;
    });

    try {
      var customerId = _booking.customer.id;
      if (customerId == null || customerId.isEmpty) {
        customerId = await _api.createCustomer(_booking.customer);
        _booking = _booking.copyWith(
          customer: _booking.customer.copyWith(id: customerId),
        );
      }

      final bookingId = await _api.confirmBooking(
        barberId: barber.id,
        serviceId: service.id,
        customerId: customerId,
        slot: slot,
      );

      if (!mounted) {
        return;
      }

      setState(() {
        _loading = false;
        _successMessage = 'Appointment confirmed. Reference: $bookingId';
        _step = 6;
      });
    } on ApiException catch (error) {
      if (!mounted) {
        return;
      }
      setState(() {
        _loading = false;
        if (error.statusCode == 409) {
          _booking = _booking.copyWith(clearSlot: true);
          _step = 3;
          _error = 'This slot was just booked. Please choose another time.';
        } else {
          _error = error.message;
        }
      });
    } catch (_) {
      if (!mounted) {
        return;
      }
      setState(() {
        _loading = false;
        _error = 'Unexpected error while confirming the booking.';
      });
    }
  }

  void _resetBooking() {
    setState(() {
      _booking = const BookingDraft();
      _step = 0;
      _services = const [];
      _slots = const [];
      _error = null;
      _successMessage = null;
    });
    _firstNameController.clear();
    _lastNameController.clear();
    _phoneController.clear();
    _emailController.clear();
    _addressController.clear();
    _loadBarbers();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Book Appointment'),
      ),
      drawer: AppDrawer(api: _api),
      body: Container(
        decoration: const BoxDecoration(
          gradient: LinearGradient(
            begin: Alignment.topCenter,
            end: Alignment.bottomCenter,
            colors: [Color(0xFFF0D8BC), Color(0xFFF7F0E6), Color(0xFFE9D7C0)],
          ),
        ),
        child: SafeArea(
          child: Center(
            child: ConstrainedBox(
              constraints: const BoxConstraints(maxWidth: 560),
              child: Padding(
                padding: const EdgeInsets.all(16),
                child: Card(
                  child: Padding(
                    padding: const EdgeInsets.all(24),
                    child: Column(
                      children: [
                        _Header(step: _step),
                        const SizedBox(height: 18),
                        if (_error != null) _MessageBanner(text: _error!, isError: true),
                        if (_successMessage != null)
                          _MessageBanner(text: _successMessage!, isError: false),
                        if (_loading)
                          const Padding(
                            padding: EdgeInsets.symmetric(vertical: 12),
                            child: LinearProgressIndicator(),
                          ),
                        Expanded(
                          child: AnimatedSwitcher(
                            duration: const Duration(milliseconds: 250),
                            child: _buildStep(),
                          ),
                        ),
                      ],
                    ),
                  ),
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildStep() {
    switch (_step) {
      case 0:
        return _BarberStep(
          key: const ValueKey('barber-step'),
          barbers: _barbers,
          onSelect: _selectBarber,
          onRetry: _loadBarbers,
        );
      case 1:
        return _ServiceStep(
          key: const ValueKey('service-step'),
          services: _services,
          barberName: _booking.barber?.name ?? '',
          onSelect: _selectService,
          onBack: () => setState(() => _step = 0),
        );
      case 2:
        return _DateStep(
          key: const ValueKey('date-step'),
          date: _booking.date,
          onPickDate: _pickDate,
          onBack: () => setState(() => _step = 1),
        );
      case 3:
        return _SlotStep(
          key: const ValueKey('slot-step'),
          slots: _slots,
          selected: _booking.slot,
          onSelect: _selectSlot,
          onReload: _loadSlots,
          onBack: () => setState(() => _step = 2),
        );
      case 4:
        return _CustomerStep(
          key: const ValueKey('customer-step'),
          formKey: _formKey,
          firstNameController: _firstNameController,
          lastNameController: _lastNameController,
          phoneController: _phoneController,
          emailController: _emailController,
          addressController: _addressController,
          onBack: () => setState(() => _step = 3),
          onNext: _saveCustomerStep,
        );
      case 5:
        return _SummaryStep(
          key: const ValueKey('summary-step'),
          booking: _booking,
          onBack: () => setState(() => _step = 4),
          onConfirm: _confirmBooking,
          isLoading: _loading,
        );
      default:
        return _SuccessStep(
          key: const ValueKey('success-step'),
          booking: _booking,
          message: _successMessage ?? 'Appointment confirmed.',
          onReset: _resetBooking,
        );
    }
  }
}

class _Header extends StatelessWidget {
  const _Header({required this.step});

  final int step;

  @override
  Widget build(BuildContext context) {
    const labels = [
      'Barber',
      'Service',
      'Date',
      'Time',
      'Customer',
      'Review',
      'Done',
    ];

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          'BarberBooking',
          style: Theme.of(context).textTheme.titleLarge?.copyWith(
                fontWeight: FontWeight.w800,
                color: const Color(0xFF1F1610),
              ),
        ),
        const SizedBox(height: 6),
        Text(
          'Mobile booking flow based on the existing web experience.',
          style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                color: const Color(0xFF6D5A49),
              ),
        ),
        const SizedBox(height: 18),
        Wrap(
          spacing: 8,
          runSpacing: 8,
          children: List.generate(labels.length, (index) {
            final active = index <= step;
            return Chip(
              label: Text(labels[index]),
              backgroundColor:
                  active ? const Color(0xFF1F1610) : const Color(0xFFF1E4D2),
              labelStyle: TextStyle(
                color: active ? Colors.white : const Color(0xFF6D5A49),
                fontWeight: FontWeight.w600,
              ),
              side: BorderSide.none,
            );
          }),
        ),
      ],
    );
  }
}

class _BarberStep extends StatelessWidget {
  const _BarberStep({
    super.key,
    required this.barbers,
    required this.onSelect,
    required this.onRetry,
  });

  final List<Barber> barbers;
  final ValueChanged<Barber> onSelect;
  final Future<void> Function() onRetry;

  @override
  Widget build(BuildContext context) {
    if (barbers.isEmpty) {
      return _EmptyState(
        title: 'No barbers available',
        subtitle: 'Start the API and seed data, then reload this screen.',
        actionLabel: 'Reload',
        onPressed: onRetry,
      );
    }

    return ListView(
      key: const ValueKey('barber-list'),
      children: [
        Text(
          'Choose your barber',
          style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                fontWeight: FontWeight.w800,
              ),
        ),
        const SizedBox(height: 8),
        Text(
          'The app is using the same /api/barbers endpoint as the web frontend.',
          style: Theme.of(context).textTheme.bodyMedium,
        ),
        const SizedBox(height: 18),
        for (final barber in barbers)
          Padding(
            padding: const EdgeInsets.only(bottom: 12),
            child: FilledButton(
              style: FilledButton.styleFrom(
                backgroundColor: const Color(0xFF1F1610),
                foregroundColor: Colors.white,
                minimumSize: const Size.fromHeight(60),
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(20),
                ),
              ),
              onPressed: () => onSelect(barber),
              child: Align(
                alignment: Alignment.centerLeft,
                child: Text(
                  barber.name,
                  style: const TextStyle(
                    fontSize: 16,
                    fontWeight: FontWeight.w700,
                  ),
                ),
              ),
            ),
          ),
      ],
    );
  }
}

class _ServiceStep extends StatelessWidget {
  const _ServiceStep({
    super.key,
    required this.services,
    required this.barberName,
    required this.onSelect,
    required this.onBack,
  });

  final List<ServiceItem> services;
  final String barberName;
  final ValueChanged<ServiceItem> onSelect;
  final VoidCallback onBack;

  @override
  Widget build(BuildContext context) {
    return ListView(
      children: [
        Text(
          'Select a service',
          style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                fontWeight: FontWeight.w800,
              ),
        ),
        const SizedBox(height: 8),
        Text('Available services for $barberName'),
        const SizedBox(height: 18),
        if (services.isEmpty)
          const _InlineInfo(text: 'No services returned for the selected barber.'),
        for (final service in services)
          Padding(
            padding: const EdgeInsets.only(bottom: 12),
            child: OutlinedButton(
              style: OutlinedButton.styleFrom(
                minimumSize: const Size.fromHeight(76),
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(20),
                ),
                side: const BorderSide(color: Color(0xFFCCB394)),
              ),
              onPressed: () => onSelect(service),
              child: Row(
                children: [
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          service.name,
                          style: const TextStyle(
                            fontWeight: FontWeight.w700,
                            color: Color(0xFF1F1610),
                          ),
                        ),
                        const SizedBox(height: 4),
                        Text('${service.durationMinutes} min'),
                      ],
                    ),
                  ),
                  Text('\$${service.price.toStringAsFixed(2)}'),
                ],
              ),
            ),
          ),
        const SizedBox(height: 8),
        TextButton(onPressed: onBack, child: const Text('Back')),
      ],
    );
  }
}

class _DateStep extends StatelessWidget {
  const _DateStep({
    super.key,
    required this.date,
    required this.onPickDate,
    required this.onBack,
  });

  final DateTime? date;
  final Future<void> Function() onPickDate;
  final VoidCallback onBack;

  @override
  Widget build(BuildContext context) {
    return ListView(
      children: [
        Text(
          'Pick a date',
          style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                fontWeight: FontWeight.w800,
              ),
        ),
        const SizedBox(height: 8),
        Text(
          date == null ? 'Select the appointment date.' : _friendlyDate(date!),
        ),
        const SizedBox(height: 20),
        FilledButton.tonal(
          onPressed: onPickDate,
          style: FilledButton.styleFrom(
            minimumSize: const Size.fromHeight(56),
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(20),
            ),
          ),
          child: Text(date == null ? 'Choose date' : 'Change date'),
        ),
        const SizedBox(height: 8),
        TextButton(onPressed: onBack, child: const Text('Back')),
      ],
    );
  }
}

class _SlotStep extends StatelessWidget {
  const _SlotStep({
    super.key,
    required this.slots,
    required this.selected,
    required this.onSelect,
    required this.onReload,
    required this.onBack,
  });

  final List<TimeSlot> slots;
  final TimeSlot? selected;
  final ValueChanged<TimeSlot> onSelect;
  final Future<void> Function() onReload;
  final VoidCallback onBack;

  @override
  Widget build(BuildContext context) {
    if (slots.isEmpty) {
      return _EmptyState(
        title: 'No available slots',
        subtitle: 'The API returned no availability for this date.',
        actionLabel: 'Reload availability',
        onPressed: onReload,
        secondaryAction: TextButton(onPressed: onBack, child: const Text('Back')),
      );
    }

    return ListView(
      children: [
        Text(
          'Choose a time',
          style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                fontWeight: FontWeight.w800,
              ),
        ),
        const SizedBox(height: 16),
        Wrap(
          spacing: 10,
          runSpacing: 10,
          children: [
            for (final slot in slots)
              ChoiceChip(
                label: Text('${_formatTime(slot.start)} - ${_formatTime(slot.end)}'),
                selected: selected?.start == slot.start,
                onSelected: slot.isAvailable ? (_) => onSelect(slot) : null,
              ),
          ],
        ),
        const SizedBox(height: 16),
        TextButton(onPressed: onBack, child: const Text('Back')),
      ],
    );
  }
}

class _CustomerStep extends StatelessWidget {
  const _CustomerStep({
    super.key,
    required this.formKey,
    required this.firstNameController,
    required this.lastNameController,
    required this.phoneController,
    required this.emailController,
    required this.addressController,
    required this.onBack,
    required this.onNext,
  });

  final GlobalKey<FormState> formKey;
  final TextEditingController firstNameController;
  final TextEditingController lastNameController;
  final TextEditingController phoneController;
  final TextEditingController emailController;
  final TextEditingController addressController;
  final VoidCallback onBack;
  final VoidCallback onNext;

  @override
  Widget build(BuildContext context) {
    return Form(
      key: formKey,
      child: ListView(
        children: [
          Text(
            'Customer details',
            style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                  fontWeight: FontWeight.w800,
                ),
          ),
          const SizedBox(height: 16),
          TextFormField(
            controller: firstNameController,
            decoration: const InputDecoration(labelText: 'First name'),
            validator: _requiredField,
          ),
          const SizedBox(height: 12),
          TextFormField(
            controller: lastNameController,
            decoration: const InputDecoration(labelText: 'Last name'),
            validator: _requiredField,
          ),
          const SizedBox(height: 12),
          TextFormField(
            controller: phoneController,
            decoration: const InputDecoration(labelText: 'Phone number'),
            keyboardType: TextInputType.phone,
            validator: _requiredField,
          ),
          const SizedBox(height: 12),
          TextFormField(
            controller: emailController,
            decoration: const InputDecoration(labelText: 'Email'),
            keyboardType: TextInputType.emailAddress,
            validator: (value) {
              if (value == null || value.trim().isEmpty) {
                return 'Required';
              }
              if (!value.contains('@')) {
                return 'Enter a valid email';
              }
              return null;
            },
          ),
          const SizedBox(height: 12),
          TextFormField(
            controller: addressController,
            decoration: const InputDecoration(labelText: 'Address'),
            maxLines: 2,
          ),
          const SizedBox(height: 16),
          FilledButton(onPressed: onNext, child: const Text('Continue')),
          const SizedBox(height: 8),
          TextButton(onPressed: onBack, child: const Text('Back')),
        ],
      ),
    );
  }

  String? _requiredField(String? value) {
    if (value == null || value.trim().isEmpty) {
      return 'Required';
    }
    return null;
  }
}

class _SummaryStep extends StatelessWidget {
  const _SummaryStep({
    super.key,
    required this.booking,
    required this.onBack,
    required this.onConfirm,
    required this.isLoading,
  });

  final BookingDraft booking;
  final VoidCallback onBack;
  final Future<void> Function() onConfirm;
  final bool isLoading;

  @override
  Widget build(BuildContext context) {
    return ListView(
      children: [
        Text(
          'Review booking',
          style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                fontWeight: FontWeight.w800,
              ),
        ),
        const SizedBox(height: 16),
        _SummaryRow(label: 'Barber', value: booking.barber?.name ?? '-'),
        _SummaryRow(label: 'Service', value: booking.service?.name ?? '-'),
        _SummaryRow(
          label: 'Price',
          value: booking.service == null
              ? '-'
              : '\$${booking.service!.price.toStringAsFixed(2)}',
        ),
        _SummaryRow(
          label: 'Duration',
          value: booking.service == null ? '-' : '${booking.service!.durationMinutes} min',
        ),
        _SummaryRow(
          label: 'Date',
          value: booking.date == null ? '-' : _friendlyDate(booking.date!),
        ),
        _SummaryRow(
          label: 'Time',
          value: booking.slot == null
              ? '-'
              : '${_formatTime(booking.slot!.start)} - ${_formatTime(booking.slot!.end)}',
        ),
        _SummaryRow(
          label: 'Customer',
          value: '${booking.customer.firstname} ${booking.customer.lastname}'.trim(),
        ),
        const SizedBox(height: 16),
        FilledButton(
          onPressed: isLoading ? null : () => onConfirm(),
          child: Text(isLoading ? 'Confirming...' : 'Confirm booking'),
        ),
        const SizedBox(height: 8),
        TextButton(onPressed: onBack, child: const Text('Back')),
      ],
    );
  }
}

class _SuccessStep extends StatelessWidget {
  const _SuccessStep({
    super.key,
    required this.booking,
    required this.message,
    required this.onReset,
  });

  final BookingDraft booking;
  final String message;
  final VoidCallback onReset;

  @override
  Widget build(BuildContext context) {
    return ListView(
      children: [
        const SizedBox(height: 24),
        const Icon(Icons.check_circle, size: 72, color: Color(0xFF2A7A44)),
        const SizedBox(height: 16),
        Center(
          child: Text(
            'Booking confirmed',
            style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                  fontWeight: FontWeight.w800,
                ),
          ),
        ),
        const SizedBox(height: 8),
        Center(child: Text(message, textAlign: TextAlign.center)),
        const SizedBox(height: 24),
        _InlineInfo(
          text:
              '${booking.barber?.name ?? ''} / ${booking.service?.name ?? ''} / ${booking.slot == null ? '' : _formatTime(booking.slot!.start)}',
        ),
        const SizedBox(height: 24),
        FilledButton.tonal(onPressed: onReset, child: const Text('Book another appointment')),
      ],
    );
  }
}

class _SummaryRow extends StatelessWidget {
  const _SummaryRow({required this.label, required this.value});

  final String label;
  final String value;

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 8),
      child: Row(
        children: [
          Expanded(
            child: Text(
              label,
              style: const TextStyle(
                fontWeight: FontWeight.w700,
                color: Color(0xFF6D5A49),
              ),
            ),
          ),
          Expanded(
            child: Text(
              value.isEmpty ? '-' : value,
              textAlign: TextAlign.right,
            ),
          ),
        ],
      ),
    );
  }
}

class _MessageBanner extends StatelessWidget {
  const _MessageBanner({required this.text, required this.isError});

  final String text;
  final bool isError;

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      margin: const EdgeInsets.only(bottom: 12),
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: isError ? const Color(0xFFFBE4E2) : const Color(0xFFE3F2E8),
        borderRadius: BorderRadius.circular(16),
      ),
      child: Text(
        text,
        style: TextStyle(
          color: isError ? const Color(0xFF9F3F36) : const Color(0xFF2A7A44),
          fontWeight: FontWeight.w600,
        ),
      ),
    );
  }
}

class _InlineInfo extends StatelessWidget {
  const _InlineInfo({required this.text});

  final String text;

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: const Color(0xFFF4E8D8),
        borderRadius: BorderRadius.circular(18),
      ),
      child: Text(text),
    );
  }
}

class _EmptyState extends StatelessWidget {
  const _EmptyState({
    required this.title,
    required this.subtitle,
    required this.actionLabel,
    required this.onPressed,
    this.secondaryAction,
  });

  final String title;
  final String subtitle;
  final String actionLabel;
  final Future<void> Function() onPressed;
  final Widget? secondaryAction;

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Text(
            title,
            style: Theme.of(context).textTheme.titleLarge?.copyWith(
                  fontWeight: FontWeight.w800,
                ),
          ),
          const SizedBox(height: 8),
          Text(subtitle, textAlign: TextAlign.center),
          const SizedBox(height: 16),
          FilledButton.tonal(onPressed: () => onPressed(), child: Text(actionLabel)),
          if (secondaryAction != null) secondaryAction ?? const SizedBox.shrink(),
        ],
      ),
    );
  }
}

String _friendlyDate(DateTime date) {
  const months = [
    'Jan',
    'Feb',
    'Mar',
    'Apr',
    'May',
    'Jun',
    'Jul',
    'Aug',
    'Sep',
    'Oct',
    'Nov',
    'Dec',
  ];
  return '${months[date.month - 1]} ${date.day}, ${date.year}';
}

String _formatTime(DateTime time) {
  final hour = time.hour.toString().padLeft(2, '0');
  final minute = time.minute.toString().padLeft(2, '0');
  return '$hour:$minute';
}
