import * as yup from 'yup';

export const registerTemplate = yup.object({
  password: yup
    .string()
    .min(7, 'Password must be at least 7 characters long')
    .matches(/[a-zA-Z]/, 'Password can only contain Latin letters.')
    .matches(/[0-9]/, 'Password must contain at least one number.')
    .matches(/[@$!%*?&]/, 'Password must contain at least one special character.')
    .matches(/^(?=.*[A-Z])(?=.*[a-z])/, 'Password must contain at least one uppercase and one lowercase letter.')
    .required('Password is required'),
  email: yup
    .string()
    .email('Invalid email format')
    .required('Email is required'),
  name: yup
    .string()
    .required('Name is required'),
  surname: yup
    .string()
    .required('Surname is required'),
});