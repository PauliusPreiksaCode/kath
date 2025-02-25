import * as yup from 'yup';

export const registerTemplate = yup.object({
  password: yup
    .string()
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