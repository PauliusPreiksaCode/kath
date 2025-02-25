import { createContext, useState, useEffect } from 'react';
import authService from './auth';

interface Props {
  children?: React.ReactNode;
}

interface UserContextProps {
  user: any;
  isLoggedIn: boolean;
  logout: () => void;
  login: (token: any) => void;
}
  

export const UserContext = createContext<UserContextProps>({
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

  const logout = () => {
    authService.removeCookies();
    setUser(null);
    sessionStorage.removeItem('user');
  };

  const login = (token : any) => {
    authService.setCookies(token);
    const currentUser = authService.getUserInfo();
    setUser(currentUser);
    sessionStorage.setItem('user', JSON.stringify(currentUser));
  };

  const UserContextValues : UserContextProps = {
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
