import 'package:flutter/material.dart';
import 'booking_api.dart';
import 'booking_page.dart';

class HomePage extends StatefulWidget {
  const HomePage({super.key, this.api});

  final BarberBookingApi? api;

  @override
  State<HomePage> createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> {
  bool _isAiProcessing = false;

  Future<void> _showAiReservationDialog() async {
    final controller = TextEditingController();
    return showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Row(
          children: [
            Icon(Icons.auto_awesome, color: Colors.deepPurple),
            SizedBox(width: 10),
            Text('LLM Reservation'),
          ],
        ),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            const Text(
              'Tell our AI what you need (e.g., "I want a haircut with John tomorrow at 10am") and we\'ll handle the rest.',
            ),
            const SizedBox(height: 16),
            TextField(
              controller: controller,
              decoration: const InputDecoration(
                hintText: 'Enter your request...',
                border: OutlineInputBorder(),
              ),
              maxLines: 3,
            ),
          ],
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Cancel'),
          ),
          ElevatedButton(
            onPressed: () async {
              final query = controller.text.trim();
              if (query.isEmpty) return;

              Navigator.pop(context);
              _processLlmRequest(query);
            },
            child: const Text('Send'),
          ),
        ],
      ),
    );
  }

  Future<void> _processLlmRequest(String query) async {
    setState(() => _isAiProcessing = true);
    try {
      final api = widget.api ?? BarberBookingApi();
      final result = await api.triggerAiReservation(
        transcript: query,
        context: 'Main Screen',
      );

      if (!mounted) return;

      final spoken = result['spokenResponse']?.toString().trim();
      final steps = (result['steps'] is List)
          ? (result['steps'] as List).whereType<dynamic>().map((e) => e.toString()).toList()
          : const <String>[];

      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(
            (spoken != null && spoken.isNotEmpty)
                ? spoken
                : (steps.isNotEmpty ? steps.join(' | ') : 'AI reservation request sent!'),
          ),
          backgroundColor: Colors.green,
        ),
      );
    } catch (e) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('AI Error: ${e.toString()}'),
          backgroundColor: Colors.red,
        ),
      );
    } finally {
      if (mounted) {
        setState(() => _isAiProcessing = false);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Barber Booking'),
      ),
      drawer: AppDrawer(api: widget.api),
      body: Container(
        decoration: const BoxDecoration(
          gradient: LinearGradient(
            begin: Alignment.topCenter,
            end: Alignment.bottomCenter,
            colors: [Color(0xFFF0D8BC), Color(0xFFF7F0E6), Color(0xFFE9D7C0)],
          ),
        ),
        child: Stack(
          children: [
            Center(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  const Icon(
                    Icons.content_cut,
                    size: 80,
                    color: Color(0xFF1F1610),
                  ),
                  const SizedBox(height: 24),
                  Text(
                    'Welcome to BarberBooking',
                    style: Theme.of(context).textTheme.headlineMedium?.copyWith(
                          fontWeight: FontWeight.w800,
                          color: const Color(0xFF1F1610),
                        ),
                  ),
                  const SizedBox(height: 16),
                  const Padding(
                    padding: EdgeInsets.symmetric(horizontal: 32),
                    child: Text(
                      'Book your next haircut with ease. Choose your favorite barber and service in just a few taps.',
                      textAlign: TextAlign.center,
                      style: TextStyle(color: Color(0xFF6D5A49), fontSize: 16),
                    ),
                  ),
                  const SizedBox(height: 40),
                  FilledButton.icon(
                    onPressed: () {
                      Navigator.of(context).push(
                        MaterialPageRoute(
                          builder: (context) => BookingPage(api: widget.api),
                        ),
                      );
                    },
                    icon: const Icon(Icons.calendar_month),
                    label: const Text('Book Now'),
                    style: FilledButton.styleFrom(
                      padding: const EdgeInsets.symmetric(horizontal: 32, vertical: 16),
                      backgroundColor: const Color(0xFF1F1610),
                    ),
                  ),
                ],
              ),
            ),
            if (_isAiProcessing)
              const Center(child: CircularProgressIndicator()),
          ],
        ),
      ),
      floatingActionButton: FloatingActionButton.extended(
        onPressed: _showAiReservationDialog,
        backgroundColor: const Color(0xFF1F1610),
        icon: const Icon(Icons.auto_awesome, color: Colors.amber),
        label: const Text('AI Booking', style: TextStyle(color: Colors.white)),
      ),
    );
  }
}

class AppDrawer extends StatelessWidget {
  const AppDrawer({super.key, this.api});

  final BarberBookingApi? api;

  @override
  Widget build(BuildContext context) {
    return Drawer(
      child: ListView(
        padding: EdgeInsets.zero,
        children: [
          DrawerHeader(
            decoration: const BoxDecoration(
              color: Color(0xFF1F1610),
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Icon(Icons.content_cut, color: Colors.white, size: 40),
                const SizedBox(height: 12),
                Text(
                  'BarberBooking',
                  style: Theme.of(context).textTheme.titleLarge?.copyWith(
                        color: Colors.white,
                        fontWeight: FontWeight.bold,
                      ),
                ),
              ],
            ),
          ),
          ListTile(
            leading: const Icon(Icons.home),
            title: const Text('Home'),
            onTap: () {
              Navigator.of(context).pop();
              Navigator.of(context).pushReplacement(
                MaterialPageRoute(builder: (context) => HomePage(api: api)),
              );
            },
          ),
          ListTile(
            leading: const Icon(Icons.calendar_month),
            title: const Text('Book Appointment'),
            onTap: () {
              Navigator.of(context).pop();
              Navigator.of(context).push(
                MaterialPageRoute(builder: (context) => BookingPage(api: api)),
              );
            },
          ),
          const Divider(),
          ListTile(
            leading: const Icon(Icons.settings),
            title: const Text('Settings'),
            onTap: () {
              Navigator.pop(context);
            },
          ),
        ],
      ),
    );
  }
}
