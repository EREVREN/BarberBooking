import 'dart:convert';
import 'dart:io';

import 'package:flutter/foundation.dart';

import 'models.dart';

class ApiException implements Exception {
  ApiException(this.message, {this.statusCode});

  final String message;
  final int? statusCode;

  @override
  String toString() => message;
}

class BarberBookingApi {
  BarberBookingApi({HttpClient? client}) : _client = client ?? HttpClient();

  final HttpClient _client;

  /// Update this with your machine's local IP address for physical device testing.
  /// Example: '192.168.1.100'  '192.168.1.148'
  static const String _localIpAddress = '10.0.2.2'; // Default for Emulator

  String get _baseUrl {
    if (kIsWeb) {
      return 'http://localhost:5273/api';
    }
    if (kReleaseMode) {
      return 'https://your-production-api.com/api'; // Your real hosted URL
    }
    // Keep your local IP logic for development
    if (Platform.isAndroid) {
      // Use the local IP address for physical devices, or 10.0.2.2 for emulator.
      return 'http://$_localIpAddress:5273/api';
    }
    return 'http://localhost:5273/api';
  }

  Future<List<Barber>> fetchBarbers() async {
    final json = await _getList('/barbers');
    return json.map(Barber.fromJson).toList();
  }

  Future<List<ServiceItem>> fetchServices(String barberId) async {
    final json = await _getList('/services/$barberId');
    return json.map(ServiceItem.fromJson).toList();
  }

  Future<List<TimeSlot>> fetchAvailability({
    required String barberId,
    required DateTime date,
    required int serviceDurationMinutes,
  }) async {
    final uri = Uri.parse(
      '$_baseUrl/availability?barberId=$barberId'
      '&date=${Uri.encodeQueryComponent(_toDateOnly(date))}'
      '&serviceDurationMinutes=$serviceDurationMinutes',
    );
    final json = await _send(uri);
    final list = json is List
        ? json
        : (json is Map<String, dynamic> ? json['slots'] as List<dynamic>? : null);
    return (list ?? const [])
        .whereType<Map<String, dynamic>>()
        .map(TimeSlot.fromJson)
        .toList();
  }

  Future<String> createCustomer(CustomerDraft customer) async {
    final json = await _post('/customers', customer.toJson());
    final id = json['id'] ?? json['Id'] ?? json['customerId'];
    if (id is! String) {
      throw ApiException('Customer created but no customer id was returned.');
    }
    return id;
  }

  Future<String> confirmBooking({
    required String barberId,
    required String serviceId,
    required String customerId,
    required TimeSlot slot,
  }) async {
    final json = await _post('/appointments/confirm', {
      'barberId': barberId,
      'serviceId': serviceId,
      'customerId': customerId,
      'startTime': slot.start.toIso8601String(),
      'endTime': slot.end.toIso8601String(),
    });
    final id = json['id'] ?? json['Id'];
    if (id is! String) {
      throw ApiException('Booking confirmed but no appointment id was returned.');
    }
    return id;
  }

  /// Trigger a voice/AI reservation request to the backend.
  ///
  /// Backend controller:
  /// POST /api/ai/reserve
  /// body: { transcript: string, context?: string }
  Future<Map<String, dynamic>> triggerAiReservation({
    required String transcript,
    String? context,
  }) async {
    return await _post('/ai/reserve', {
      'transcript': transcript,
      'context': context,
    });
  }

  /// Back-compat for older UI code that was calling `/llmapi`.
  /// The current backend route is `/api/ai/reserve`.
  @Deprecated('Use triggerAiReservation(transcript: ..., context: ...) instead.')
  Future<Map<String, dynamic>> triggerLlmApi(String query) async {
    return triggerAiReservation(transcript: query, context: 'Main Screen');
  }

  Future<List<Map<String, dynamic>>> _getList(String path) async {
    final json = await _send(Uri.parse('$_baseUrl$path'));
    if (json is! List) {
      throw ApiException('Unexpected API response.');
    }
    return json.whereType<Map<String, dynamic>>().toList();
  }

  Future<Map<String, dynamic>> _post(
    String path,
    Map<String, dynamic> body,
  ) async {
    final uri = Uri.parse('$_baseUrl$path');
    final request = await _client.postUrl(uri);
    request.headers.contentType = ContentType.json;
    request.write(jsonEncode(body));
    final response = await request.close();
    final json = await _decodeResponse(response);
    if (json is! Map<String, dynamic>) {
      throw ApiException('Unexpected API response.');
    }
    return json;
  }

  Future<dynamic> _send(Uri uri) async {
    final request = await _client.getUrl(uri);
    final response = await request.close();
    return _decodeResponse(response);
  }

  Future<dynamic> _decodeResponse(HttpClientResponse response) async {
    final body = await response.transform(utf8.decoder).join();
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException(
        body.isEmpty ? 'Request failed.' : body,
        statusCode: response.statusCode,
      );
    }
    if (body.isEmpty) {
      return <String, dynamic>{};
    }
    return jsonDecode(body);
  }

  String _toDateOnly(DateTime date) {
    final safe = DateTime(date.year, date.month, date.day);
    final month = safe.month.toString().padLeft(2, '0');
    final day = safe.day.toString().padLeft(2, '0');
    return '${safe.year}-$month-$day';
  }
}
