const API_BASE_URL = 'http://localhost:5273/api';

async function request(url, options = {}) {
  const response = await fetch(`${API_BASE_URL}${url}`, {
    headers: {
      'Content-Type': 'application/json',
      ...options.headers
    },
    ...options
  });

  if (!response.ok) {
    const text = await response.text();
    throw new Error(text || 'API error');
  }

  return response.json();
}

export default {
  get: (url) => request(url),
  post: (url, body) =>
    request(url, {
      method: 'POST',
      body: JSON.stringify(body)
    })
};