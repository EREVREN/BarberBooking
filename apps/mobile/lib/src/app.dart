import 'package:flutter/material.dart';

import 'booking_api.dart';
import 'home_page.dart';

class BarberBookingApp extends StatelessWidget {
  const BarberBookingApp({super.key, this.api});

  final BarberBookingApi? api;

  @override
  Widget build(BuildContext context) {
    final colorScheme = ColorScheme.fromSeed(
      seedColor: const Color(0xFFB77935),
      brightness: Brightness.light,
      primary: const Color(0xFF1F1610),
      secondary: const Color(0xFFC58E4E),
      surface: const Color(0xFFF8F1E7),
    );

    return MaterialApp(
      debugShowCheckedModeBanner: false,
      title: 'Barber Booking',
      theme: ThemeData(
        colorScheme: colorScheme,
        scaffoldBackgroundColor: const Color(0xFFF3E7D7),
        useMaterial3: true,
        appBarTheme: const AppBarTheme(
          backgroundColor: Colors.transparent,
          foregroundColor: Color(0xFF1F1610),
          elevation: 0,
        ),
        cardTheme: const CardThemeData(
          color: Color(0xEBFFFFFF),
          elevation: 0,
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.all(Radius.circular(28)),
          ),
        ),
        inputDecorationTheme: InputDecorationTheme(
          filled: true,
          fillColor: Colors.white,
          border: OutlineInputBorder(
            borderRadius: BorderRadius.circular(18),
            borderSide: const BorderSide(color: Color(0xFFDBC5AA)),
          ),
          enabledBorder: OutlineInputBorder(
            borderRadius: BorderRadius.circular(18),
            borderSide: const BorderSide(color: Color(0xFFDBC5AA)),
          ),
          focusedBorder: OutlineInputBorder(
            borderRadius: BorderRadius.circular(18),
            borderSide: const BorderSide(color: Color(0xFFC58E4E), width: 1.4),
          ),
        ),
      ),
      home: HomePage(api: api),
    );
  }
}
