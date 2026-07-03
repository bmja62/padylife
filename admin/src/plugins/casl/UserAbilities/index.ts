import type {UserAbility} from '../AppAbility'

const allAbilities: UserAbility[] = [
  {
    action: 'manage',
    subject: 'all',
  },
]
const specialistAbilities: UserAbility[] = [
  {
    action: 'manage',
    subject: 'specialist',
  },
  {
    action: 'read',
    subject: 'specialist',
  },
]

export interface IUserAbilities {
  Admin: UserAbility[],
}

export const userAbilities: IUserAbilities = {
  Admin: allAbilities,
  Specialist: specialistAbilities,
}
