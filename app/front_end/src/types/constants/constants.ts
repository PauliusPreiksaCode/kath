/**
 * Helper function to get environment variables at runtime or buildtime
 * 
 * @param key - The environment variable key
 * @param defaultValue - Default value if the environment variable is not found
 * @returns The environment variable value
 */
const getEnvVariable = (key: string, defaultValue: string = ''): string => {
    // First check runtime variables (from Docker)
    if (typeof window !== 'undefined' && window.env && window.env[key]) {
      return window.env[key];
    }
    // Fallback to Vite's import.meta.env
    return import.meta.env[key] || defaultValue;
  };
  
  /**
   * API_URL is the base URL for API requests, loaded from environment variables.
   *
   * @description This constant provides the base URL used for making HTTP requests to the API. It is loaded from the environment
   * variable `VITE_API_URL`, which should be set in the environment configuration file. The `API_URL` is used to construct full
   * URLs for API requests throughout the application.
   *
   * @constant {string} API_URL - The base URL for API requests.
   *
   * @example
   * // Example usage of API_URL
   * const endpoint = `${API_URL}/endpoint`;
   */
  export const API_URL = getEnvVariable('VITE_API_URL', 'http://localhost:8080/api/v1');
  
  /**
   * SOCKET_URL is the URL for connecting to the WebSocket server, loaded from environment variables.
   *
   * @description This constant provides the URL used for establishing a WebSocket connection. It is loaded from the environment
   * variable `VITE_SOCKET_URL`, which should be set in the environment configuration file. The `SOCKET_URL` is used to initialize
   * the WebSocket client for real-time communication.
   *
   * @constant {string} SOCKET_URL - The URL for connecting to the WebSocket server.
   *
   * @example
   * // Example usage of SOCKET_URL
   * const socket = io(SOCKET_URL);
   */
  export const SOCKET_URL = getEnvVariable('VITE_SOCKET_URL', 'http://localhost:8080');
  
  /**
   * ORGANIZATION_API_URL is the base URL for organization-related API requests.
   *
   * @description This constant provides the base URL used for making HTTP requests to the organization API. 
   * It is loaded from the environment variable `VITE_ORGANIZATION_API_URL`.
   *
   * @constant {string} ORGANIZATION_API_URL - The base URL for organization API requests.
   */
  export const ORGANIZATION_API_URL = getEnvVariable('VITE_ORGANIZATION_API_URL', 'https://localhost:7059');
  
  /**
   * ORGANIZATION_SOCKET_URL is the URL for connecting to the organization WebSocket server.
   *
   * @description This constant provides the URL used for establishing a WebSocket connection to the organization service.
   * It is loaded from the environment variable `VITE_ORGANIZATION_SOCKET_URL`.
   *
   * @constant {string} ORGANIZATION_SOCKET_URL - The URL for connecting to the organization WebSocket server.
   */
  export const ORGANIZATION_SOCKET_URL = getEnvVariable('VITE_ORGANIZATION_SOCKET_URL', 'https://localhost:7059');
  
  declare global {
    interface Window {
      env?: {
        [key: string]: string;
      };
    }
  }