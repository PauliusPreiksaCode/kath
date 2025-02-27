import { createContext, useState, useEffect } from 'react';
import authService from './auth';
import { logOutUser } from './api';

interface Props {
  children?: React.ReactNode;
}

interface UserContextProps {
  userId: string | null;
  roles: string[] | null;
  user: any;
  isLoggedIn: boolean;
  logout: () => void;
  login: (token: any) => void;
}
  

export const UserContext = createContext<UserContextProps>({
    userId: null,
    roles: null,
    user: null,
    isLoggedIn: false,
    logout: () => {},
    login: () => {},
});

const AuthenticationProvider : React.FC<Props> = ({ children }) => {

  const storedUser = JSON.parse(String(sessionStorage.getItem('user')));
  const [user, setUser] = useState(storedUser || null);

  useEffect(() => {
    const currentUser = authService.getUserInfo();
    setUser(currentUser);
    sessionStorage.setItem('user', JSON.stringify(currentUser));
  }, []);

  const isLoggedIn = !!user;
  const roles = user?.decodedJwt?.["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
  const userId = user?.decodedJwt?.sub;

  const logout = async () => {
    authService.removeCookies();
    setUser(null);
    sessionStorage.removeItem('user');
    await logOutUser();
  };

  const login = (token : any) => {
    authService.setCookies(token);
    const currentUser = authService.getUserInfo();
    setUser(currentUser);
    sessionStorage.setItem('user', JSON.stringify(currentUser));
  };

  const UserContextValues : UserContextProps = {
    userId,
    roles,
    user,
    isLoggedIn,
    logout,
    login,
  };

  return (
    <UserContext.Provider value ={UserContextValues}>
      {children}
    </UserContext.Provider>
  );
}

export default AuthenticationProvider;
