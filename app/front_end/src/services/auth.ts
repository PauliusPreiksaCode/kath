import Cookies from 'js-cookie';
import axios from 'axios';
import { ORGANIZATION_API_URL } from '@/types';

async function renewToken () {
    try {
      const result = await axios.post(
        `${ORGANIZATION_API_URL}/auth/accessToken`,
        {},
        { withCredentials: true }
      );
  
      const token = result.data;
      setCookies(token);
      return getUserInfo();
    } catch (error) {
      removeCookies();
      window.location.href = '/login';
      return null;
    }
  }
  
  function setCookies(cookie1: any) {
    Cookies.set('jwtToken', cookie1);
  }
  
  function removeCookies(){
    Cookies.remove('jwtToken');
    Cookies.remove('RefreshToken');
  }
  
  function getUserInfo() {
    const token = Cookies.get('jwtToken');
  
    if (token) {
      let payloadBase64 = token.split('.')[1];
      let base64 = payloadBase64.replace(/-/g, '+').replace(/_/g, '/');
      const decodedJwt = JSON.parse(window.atob(base64));
  
      return {decodedJwt};
    }
    return null;
  }
  
  function getToken() {
    return Cookies.get('jwtToken');
  }
  
  const authService = {
    renewToken,
    setCookies,
    removeCookies,
    getUserInfo,
    getToken,
  };
  
  export default authService;