export class ApiRequestError extends Error {
  constructor(readonly status: number) {
    super(`API request failed with HTTP ${status}.`);
    this.name = "ApiRequestError";
  }
}

export async function requestJson<T>(
  input: RequestInfo | URL,
  init?: RequestInit,
): Promise<T> {
  const response = await fetch(input, init);
  if (!response.ok) {
    throw new ApiRequestError(response.status);
  }

  return (await response.json()) as T;
}
