import authService from '@/services/auth';
import { API_URL, ORGANIZATION_API_URL } from '@/types';
import { getUUID } from '@/utils';
import axios, { AxiosInstance } from 'axios';

/**
 * axiosInstance is a configured Axios instance for making HTTP requests.
 *
 * @description This instance of Axios is pre-configured with a base URL, request timeout, and default headers. It sets up
 * a base URL for all requests using the `API_URL` constant and a timeout of 60 seconds. The default headers include `Content-Type`
 * set to `application/json` and a unique UUID for tracking or identifying requests.
 *
 * The `getUUID` utility function is used to generate a unique identifier, which is included in the headers to assist with request
 * tracking or other purposes.
 *
 * @constant {AxiosInstance} axiosInstance - The configured Axios instance for making HTTP requests.
 *
 * @example
 * // Example usage of axiosInstance
 * axiosInstance.get('/endpoint')
 *   .then(response => console.log(response))
 *   .catch(error => console.error(error));
 */
export const axiosInstance: AxiosInstance = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
    uuid: getUUID(),
  },
});

const token = authService.getToken();

const instance = axios.create({
  baseURL: ORGANIZATION_API_URL,
  withCredentials: true,
  headers: {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}`,
  },
});

instance.interceptors.request.use(
  async (config) => {
    const token = await authService.getToken();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

instance.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;
    
    if (error.response && error.response.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      const newToken = await handleSessionExpired();
      if (newToken) {
        originalRequest.headers['Authorization'] = `Bearer ${newToken}`;
        return instance(originalRequest);
      }
    }

    return Promise.reject(error);
  }
);

const handleSessionExpired = async () => {
  const newToken = await authService.renewToken();
  return newToken;
};

export default instance;