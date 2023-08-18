declare module "cms-types" {
    export interface IEntity {
      id: string;
      createdAt: Date;
      updatedAt: Date;
    }

    export interface Guest extends IEntity {
      firstName: string;
      lastName: string;
      email: string;
      roomId: string;
    }
}
