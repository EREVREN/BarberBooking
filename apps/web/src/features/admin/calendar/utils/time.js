export function timeToPixels(dateTime) {
  const date = new Date(dateTime);
  return (date.getHours() * 60 + date.getMinutes()) * 2;
}

export function durationToPixels(start, end) {
  return (new Date(end) - new Date(start)) / 60000 * 2;
}