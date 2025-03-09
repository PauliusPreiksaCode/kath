import { BaseLayout } from '@/components/layouts/baseLayout';
import { Paths } from '@/types';
import { useMemo } from 'react';
import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import Login from '@/features/auth/login/Login';
import Register from '@/features/auth/register/Register';
import Organization from '@/features/organization/OrganizationList';
import Group from '@/features/group/GroupList';
import User from '@/features/user/user';
import ProcessPayment from '@/features/user/ProcessPayment';
import PaymentSuccess from '@/features/user/PaymentSuccess';
import PaymentFailed from '@/features/user/PaymentFailed';
import PrivateRoute from './routes/PrivateRoute';

/**
 * `AppRouter` component sets up the routing for the application using `createBrowserRouter` from `react-router-dom`.
 *
 * @description This component defines the routes for the application and configures lazy loading for route components.
 * It uses the `createBrowserRouter` function to set up routing, with the paths and associated components being loaded
 * asynchronously. The router configuration includes:
 * - A route for the home page, which is lazily loaded from `./routes/home`.
 * - A catch-all route for 404 pages, which is lazily loaded from `./routes/notFound`.
 *
 * The component uses `RouterProvider` to provide the router to the application, ensuring that routing is managed
 * throughout the app.
 *
 * @component
 *
 * @example
 * // Example usage of the AppRouter component
 * return (
 *   <AppRouter />
 * );
 *
 * @returns {JSX.Element} The RouterProvider component with the configured browser router.
 */
export const AppRouter = () => {
  const router = useMemo(
    () =>
      createBrowserRouter([
        {
          path: Paths.HOME,
          lazy: async () => {
            const { Home } = await import('./routes/home');
            return {
              Component: (props) => (
                <BaseLayout>
                  <Home {...props} />
                </BaseLayout>
              ),
            };
          },
        },
        {
          path: Paths.LOGIN,
          lazy: async () => {
            return {
              Component: (props) => (
                <BaseLayout>
                  <Login {...props} />
                </BaseLayout>
              ),
            };
          },
        },
        {
          path: Paths.REGISTER,
          lazy: async () => {
            return {
              Component: (props) => (
                <BaseLayout>
                  <Register {...props} />
                </BaseLayout>
              ),
            };
          },
        },
        {
          path: Paths.ORGANIZATION,
          element: <PrivateRoute allowedRoles={['LicencedUser', 'OrganizationOwner']} />,
          lazy: async () => {
            return {
              Component: (props) => (
                <BaseLayout>
                  <Organization {...props} />
                </BaseLayout>
              ),
            };
          }
        },
        {
          path: Paths.GROUP,
          element: <PrivateRoute allowedRoles={['LicencedUser', 'OrganizationOwner']} />,
          lazy: async () => {
            return {
              Component: (props) => (
                <BaseLayout>
                  <Group {...props} />
                </BaseLayout>
              ),
            };
          }
        },
        {
          path: Paths.USER,
          element: <PrivateRoute allowedRoles={['User', 'LicencedUser', 'OrganizationOwner']} />,
          lazy: async () => {
            return {
              Component: (props) => (
                <BaseLayout>
                  <User {...props} />
                </BaseLayout>
              ),
            };
          }
        },
        {
          path: Paths.PROCESS_PAYMENT,
          element: <PrivateRoute allowedRoles={['User', 'LicencedUser', 'OrganizationOwner']} />,
          lazy: async () => {
            return {
              Component: (props) => (
                <BaseLayout>
                  <ProcessPayment {...props} />
                </BaseLayout>
              ),
            };
          }
        },
        {
          path: Paths.PAYMENT_SUCCESS,
          element: <PrivateRoute allowedRoles={['User', 'LicencedUser', 'OrganizationOwner']} />,
          lazy: async () => {
            return {
              Component: (props) => (
                <BaseLayout>
                  <PaymentSuccess {...props} />
                </BaseLayout>
              ),
            };
          }
        },
        {
          path: Paths.PAYMENT_FAILED,
          element: <PrivateRoute allowedRoles={['User', 'LicencedUser', 'OrganizationOwner']} />,
          lazy: async () => {
            return {
              Component: (props) => (
                <BaseLayout>
                  <PaymentFailed {...props} />
                </BaseLayout>
              ),
            };
          }
        },
        {
          path: Paths.NOTFOUND,
          lazy: async () => {
            const { NotFound } = await import('./routes/notFound');
            return {
              Component: (props) => (
                <BaseLayout>
                  <NotFound {...props} />
                </BaseLayout>
              ),
            };
          },
        },
      ]),
    []
  );

  return <RouterProvider router={router} />;
};
